﻿using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using SME.SR.Data.Interfaces.ElasticSearch;
using SME.SR.Infra;
using SME.SR.Infra.Dtos;
using SME.SR.Infra.Utilitarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterDadosRelatorioAcompanhamentoFrequenciaCommandHandler : IRequestHandler<ObterDadosRelatorioAcompanhamentoFrequenciaCommand, RelatorioFrequenciaIndividualDto>
    {
        private readonly IMediator mediator;
        private readonly IAlunoRepository alunoRepository;
        private readonly IRepositorioElasticTurma repositorioElasticTurma;
        private IEnumerable<RelatorioFrequenciaIndividualDiariaAlunoDto> frequenciasDiarias = null;
        private IEnumerable<AusenciaBimestreDto> dadosAusencia = null;
        public ObterDadosRelatorioAcompanhamentoFrequenciaCommandHandler(IMediator mediator, IAlunoRepository alunoRepository, IRepositorioElasticTurma repositorioElasticTurma)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this.alunoRepository = alunoRepository ?? throw new ArgumentNullException(nameof(alunoRepository));
            this.repositorioElasticTurma = repositorioElasticTurma ?? throw new ArgumentNullException(nameof(repositorioElasticTurma));
        }

        public async Task<RelatorioFrequenciaIndividualDto> Handle(
            ObterDadosRelatorioAcompanhamentoFrequenciaCommand request, CancellationToken cancellationToken)
        {
            var relatorio = new RelatorioFrequenciaIndividualDto();
            var alunosSelecionados = new List<AlunoNomeDto>();
            var turma = await ObterDadosDaTurma(request.FiltroRelatorio.TurmaCodigo);

            var componentesIds = new List<string> { request.FiltroRelatorio.ComponenteCurricularId };
            
            await MapearCabecalho(relatorio, request.FiltroRelatorio, turma);
            var tipoCalendarioId = await mediator.Send(new ObterTipoCalendarioIdPorTurmaQuery(turma));
            if (tipoCalendarioId == default)
                throw new NegocioException("O tipo de calendário da turma não foi encontrado.");

            var periodosEscolares =
                await mediator.Send(new ObterPeriodosEscolaresPorTipoCalendarioQuery(tipoCalendarioId));

            int aulasDadas = await mediator.Send(new ObterAulasDadasNoBimestreQuery(turma.Codigo, tipoCalendarioId,
                componentesIds.ToArray(),
                int.Parse(request.FiltroRelatorio.Bimestre)));

            var bimestre = int.Parse(request.FiltroRelatorio.Bimestre);

            var ehPorBimestre = bimestre > 0;

            var bimestres = ehPorBimestre ? new[] { bimestre } : turma.ModalidadeCodigo.EhSemestral() ? new[] { 1, 2 } : new[] { 1, 2, 3, 4 };

            relatorio.ehTodosBimestre = !ehPorBimestre;
            relatorio.ImprimirFrequenciaDiaria = request.FiltroRelatorio.ImprimirFrequenciaDiaria;

            foreach (var bimestreItem in bimestres)
            {
                if (request.FiltroRelatorio.AlunosCodigos.Contains("-99"))
                {
                    var periodo = periodosEscolares.Where(periodoEscolar => periodoEscolar.Bimestre == bimestreItem).FirstOrDefault();
                    var alunos = await mediator.Send(new ObterAlunosTurmaPorTurmaDataSituacaoMatriculaQuery(request.FiltroRelatorio.TurmaCodigo, periodo.PeriodoFim));

                    if (turma.AnoLetivo == DateTime.Now.Year)
                        alunos = alunos.Where(a => a.DeveMostrarNaChamada(periodo.PeriodoFim, periodo.PeriodoInicio));

                    if (alunos == null && !alunos.Any())
                        throw new NegocioException("Alunos não encontrados.");

                    alunosSelecionados = new List<AlunoNomeDto>();

                    foreach (var aluno in alunos.OrderBy(x => x.NomeAluno))
                    {
                        alunosSelecionados.Add(new AlunoNomeDto
                        {
                            NomeSocial = aluno.NomeSocialAluno,
                            Nome = aluno.NomeAluno,
                            Codigo = aluno.CodigoAluno.ToString()
                        });
                    }
                }
                else
                    alunosSelecionados =
                        (await alunoRepository.ObterNomesAlunosPorCodigos(request.FiltroRelatorio.AlunosCodigos.ToArray()))
                        .ToList();

                var alunosForaBimestre = new List<string>();

                foreach(var aluno in alunosSelecionados)
                {
                    var matricula = await repositorioElasticTurma.ObterMatriculasAlunoNaTurma(int.Parse(aluno.Codigo), int.Parse(request.FiltroRelatorio.TurmaCodigo));

                    var periodoCorrespondente = periodosEscolares.Single(p => p.Bimestre == bimestreItem);

                    if (matricula.Equals(default) || matricula.dataMatricula.HasValue ? matricula.dataMatricula.Value.Date > periodoCorrespondente.PeriodoFim.Date : false)
                        alunosForaBimestre.Add(aluno.Codigo);
                }

                if (alunosSelecionados != null && alunosSelecionados.Any())
                {
                    var codigosAlunos = alunosSelecionados
                        .Select(s => s.Codigo)
                        .Except(alunosForaBimestre)
                        .ToArray();

                    var dadosFrequencia = (await mediator.Send(new ObterFrequenciaAlunoPorCodigoBimestreQuery(
                        bimestreItem.ToString(), codigosAlunos, request.FiltroRelatorio.TurmaCodigo,
                        TipoFrequenciaAluno.PorDisciplina, componentesIds.ToArray()))).ToList();

                    if (request.FiltroRelatorio.ImprimirFrequenciaDiaria)
                        frequenciasDiarias = await mediator.Send(new ObterFrequenciaAlunoDiariaQuery(
                            request.FiltroRelatorio.Bimestre, codigosAlunos, request.FiltroRelatorio.TurmaCodigo,
                            componentesIds.ToArray()));
                    else
                        dadosAusencia = await mediator.Send(new ObterAusenciaPorAlunoTurmaBimestreQuery(
                            alunosSelecionados.Select(s => s.Codigo).ToArray(), request.FiltroRelatorio.TurmaCodigo,
                            request.FiltroRelatorio.Bimestre, componentesIds.ToArray()));

                    await MapearAlunos(alunosSelecionados, relatorio, dadosFrequencia, turma, periodosEscolares, aulasDadas, bimestreItem);
                }
            }

            var relatorioFinal = new RelatorioFrequenciaIndividualDto()
            {
                ehInfantil = relatorio.ehInfantil,
                Usuario = relatorio.Usuario,
                ComponenteNome = relatorio.ComponenteNome,
                DreNome = relatorio.DreNome,
                ehTodosBimestre = relatorio.ehTodosBimestre,
                RF = relatorio.RF,
                TurmaNome = relatorio.TurmaNome,
                UeNome = relatorio.UeNome,
                ImprimirFrequenciaDiaria = relatorio.ImprimirFrequenciaDiaria,
                Alunos = new List<RelatorioFrequenciaIndividualAlunosDto>(),
            };

            var agrupamentoAlunos = relatorio.Alunos.OrderBy(p => p.NomeAluno).GroupBy(o => o.CodigoAluno);
            foreach (var agrupamentoAluno in agrupamentoAlunos)
            {
                var frequenciaAluno = new FrequenciaAluno()
                { 
                    TotalAulas = agrupamentoAluno.Sum(s => s.TotalAulasDadasFinal),
                    TotalAusencias = agrupamentoAluno.Sum(s => s.TotalAusenciasFinal),
                    TotalCompensacoes = agrupamentoAluno.Sum(s => s.TotalCompensacoesFinal),
                    TotalPresencas = agrupamentoAluno.Sum(s => s.TotalPresencasFinal),
                    TotalRemotos = agrupamentoAluno.Sum(s => s.TotalRemotoFinal),
                };
                var dadosFrequencia = agrupamentoAluno.FirstOrDefault();
                if (dadosFrequencia != null)
                {
                    dadosFrequencia.TotalAusenciasFinal = frequenciaAluno.TotalAusencias;
                    dadosFrequencia.TotalCompensacoesFinal = frequenciaAluno.TotalCompensacoes;
                    dadosFrequencia.TotalPresencasFinal = frequenciaAluno.TotalPresencas;
                    dadosFrequencia.TotalRemotoFinal = frequenciaAluno.TotalRemotos;
                    dadosFrequencia.TotalAulasDadasFinal = frequenciaAluno.TotalAulas;
                    dadosFrequencia.PercentualFrequenciaFinal = frequenciaAluno.PercentualFrequencia;
                    dadosFrequencia.Bimestres = agrupamentoAluno.SelectMany(s => s.Bimestres).ToList();
                }
                relatorioFinal.Alunos.Add(dadosFrequencia);
            }

            return relatorioFinal;
        }

        private void MapearBimestre(IEnumerable<FrequenciaAlunoConsolidadoDto> dadosFrequenciaDto, RelatorioFrequenciaIndividualAlunosDto aluno)
        {
            if (dadosFrequenciaDto != null && dadosFrequenciaDto.Any())
            {

                foreach (var item in dadosFrequenciaDto)
                {
                    if (aluno.CodigoAluno == item.CodigoAluno)
                    {
                        var bimestre = new RelatorioFrequenciaIndividualBimestresDto
                        {
                            NomeBimestre = item.BimestreFormatado,
                        };

                        var frequenciaAluno = new FrequenciaAluno() { 
                            TotalAulas = item.TotalAula,
                            TotalPresencas = item.TotalPresencas,
                            TotalRemotos = item.TotalRemotos,
                            TotalAusencias = item.TotalAusencias,
                            TotalCompensacoes = item.TotalCompensacoes,
                        };

                        bimestre.DadosFrequencia = new RelatorioFrequenciaIndividualDadosFrequenciasDto
                        {
                            TotalAulasDadas = frequenciaAluno.TotalAulas,
                            TotalPresencas = frequenciaAluno.TotalPresencas,
                            TotalRemoto = frequenciaAluno.TotalRemotos,
                            TotalAusencias = frequenciaAluno.TotalAusencias,
                            TotalCompensacoes = frequenciaAluno.TotalCompensacoes,
                            TotalPercentualFrequencia = Math.Round(frequenciaAluno.PercentualFrequencia, 0).ToString(),
                            TotalPercentualFrequenciaFormatado = frequenciaAluno.PercentualFrequenciaFormatado
                        };

                        bimestre.FrequenciaDiaria.AddRange(ObterJustificativaFrequenciaDiaria(item.Bimestre, item.CodigoAluno));

                        if (dadosAusencia != null && dadosAusencia.Any())
                        {
                            foreach (var ausencia in dadosAusencia)
                            {
                                if (item.CodigoAluno == ausencia.CodigoAluno && item.Bimestre == ausencia.Bimestre)
                                {
                                    bimestre.FrequenciaDiaria.Add(new RelatorioFrequenciaIndividualJustificativasDto
                                    {
                                        DataAula = ausencia.DataAusencia.ToString("dd/MM/yyyy"),
                                        Justificativa = UtilHtml.FormatarHtmlParaTexto(ausencia.MotivoAusencia),
                                    });
                                }
                            }
                        }
                        aluno.Bimestres.Add(bimestre);
                    }

                }
                aluno.TotalAulasDadasFinal = aluno.Bimestres.Sum(x => x.DadosFrequencia.TotalAulasDadas);
                aluno.TotalAusenciasFinal = aluno.Bimestres.Sum(x => x.DadosFrequencia.TotalAusencias);
                aluno.TotalCompensacoesFinal = aluno.Bimestres.Sum(x => x.DadosFrequencia.TotalCompensacoes);
                aluno.TotalPresencasFinal = aluno.Bimestres.Sum(x => x.DadosFrequencia.TotalPresencas);
                aluno.TotalRemotoFinal = aluno.Bimestres.Sum(x => x.DadosFrequencia.TotalRemoto);
                aluno.TituloFinal = $"FINAL - {dadosFrequenciaDto.Where(x => x.CodigoAluno == aluno.CodigoAluno).FirstOrDefault().AnoBimestre}";
                aluno.PercentualFrequenciaFinal = aluno.Bimestres.Average(x => long.Parse(x.DadosFrequencia.TotalPercentualFrequencia));
            }
        }

        private IEnumerable<RelatorioFrequenciaIndividualJustificativasDto> ObterJustificativaFrequenciaDiaria(int bimestre, string codigoAluno)
        {
            if (frequenciasDiarias != null)
            {
                var frequenciaDiariaAluno = frequenciasDiarias.ToList().FindAll(diaria =>
                                            diaria.Bimestre == bimestre &&
                                            diaria.AlunoCodigo == codigoAluno);

                return frequenciaDiariaAluno.Select(diaria => new RelatorioFrequenciaIndividualJustificativasDto
                {
                    DataAula = diaria.DataAula.ToString("dd/MM/yyyy"),
                    Justificativa = String.IsNullOrEmpty(diaria.Motivo) ? String.Empty : UtilHtml.FormatarHtmlParaTexto(diaria.Motivo),
                    QuantidadeAulas = diaria.QuantidadeAulas,
                    QuantidadeAusencia = diaria.QuantidadeAusencia,
                    QuantidadePresenca = diaria.QuantidadePresenca,
                    QuantidadeRemoto = diaria.QuantidadeRemoto,
                });
            }

            return Enumerable.Empty<RelatorioFrequenciaIndividualJustificativasDto>();
        }

        private async Task MapearCabecalho(RelatorioFrequenciaIndividualDto relatorio, FiltroAcompanhamentoFrequenciaJustificativaDto filtroRelatorio, Turma turma)
        {
            var dadosDreUe = await ObterNomeDreUe(filtroRelatorio.TurmaCodigo);

            relatorio.RF = filtroRelatorio.UsuarioRF;
            relatorio.Usuario = filtroRelatorio.UsuarioNome;
            relatorio.DreNome = dadosDreUe.DreNome;
            relatorio.UeNome = dadosDreUe.UeNome;
            relatorio.ComponenteNome = await ObterNomeComponente(turma.Codigo, filtroRelatorio.ComponenteCurricularId);
            relatorio.TurmaNome = turma.NomePorFiltroModalidade(null);
            relatorio.ehInfantil = turma != null && turma.ModalidadeCodigo == Modalidade.Infantil;
        }
        private async Task<Turma> ObterDadosDaTurma(string turmaCodigo)
            => await mediator.Send(new ObterTurmaPorCodigoQuery(turmaCodigo));

        private async Task<string> ObterNomeComponente(string codigoTurma, string componenteCodigo)
        {
            var dadosComponente = await mediator.Send(new ObterComponentesCurricularesEolPorIdsQuery(new long[] { Convert.ToInt64(componenteCodigo) }, new string[] { codigoTurma }));

            if (dadosComponente != null && dadosComponente.Any())
                return dadosComponente.FirstOrDefault().Disciplina;

            return await mediator.Send(new ObterNomeComponenteCurricularPorIdQuery(Convert.ToInt64(componenteCodigo)));
        }

        private async Task MapearAlunos(IEnumerable<AlunoNomeDto> alunos, RelatorioFrequenciaIndividualDto relatorio, List<FrequenciaAlunoConsolidadoDto> dadosFrequenciaDto, Turma turma, IEnumerable<PeriodoEscolar> periodosEscolares, int aulasDadas, int bimestre)
        {
            foreach (var aluno in alunos.OrderBy(x => x.Nome))
            {
                var descricaoSituacaoAluno = string.Empty;

                var situacaoAluno = await alunoRepository
                    .ObterAlunosPorTurmaCodigoParaRelatorioAcompanhamentoAprendizagem(long.Parse(turma.Codigo), long.Parse(aluno.Codigo), turma.AnoLetivo);

                if (situacaoAluno != null && situacaoAluno.Any())
                {
                    var dataUltimaSituacao = situacaoAluno.Max(a => a.DataSituacao);

                    var ultimaSituacaoAluno = situacaoAluno.Where(x => x.DataSituacao == dataUltimaSituacao).FirstOrDefault();

                    var ehInfantil = turma != null && turma.ModalidadeCodigo == Modalidade.Infantil;

                    descricaoSituacaoAluno = ultimaSituacaoAluno.Ativo ? string.Empty : await ObterSituacao(ultimaSituacaoAluno, ehInfantil, periodosEscolares, turma);
                }

                var relatorioFrequenciaIndividualAlunosDto = new RelatorioFrequenciaIndividualAlunosDto
                {
                    NomeAluno = $"{(aluno.NomeSocial ?? aluno.Nome)} ({aluno.Codigo}) {descricaoSituacaoAluno}",
                    CodigoAluno = aluno.Codigo
                };

                if (!dadosFrequenciaDto.Any(a => a.Bimestre == bimestre && a.CodigoAluno.Equals(aluno.Codigo)))
                {
                    dadosFrequenciaDto.Add(new FrequenciaAlunoConsolidadoDto()
                    {
                        Bimestre = bimestre,
                        CodigoAluno = aluno.Codigo,
                        AnoBimestre = turma.AnoLetivo.ToString(),
                        TotalAula = 0,
                        TotalPresencas = 0,
                        TotalRemotos = 0,
                        TotalAusencias = 0,
                        TotalCompensacoes = 0
                    });
                }

                if (dadosFrequenciaDto != null && dadosFrequenciaDto.Where(x => x.CodigoAluno == aluno.Codigo).Any())
                {
                    if (relatorio != null)
                        MapearBimestre(dadosFrequenciaDto, relatorioFrequenciaIndividualAlunosDto);

                    relatorio.Alunos.Add(relatorioFrequenciaIndividualAlunosDto);
                }
                else
                {
                    var NomeBimestre = $"{bimestre}º Bimestre";
                    relatorioFrequenciaIndividualAlunosDto.Bimestres.Add(new RelatorioFrequenciaIndividualBimestresDto()
                    {
                        NomeBimestre = NomeBimestre,
                        DadosFrequencia = new RelatorioFrequenciaIndividualDadosFrequenciasDto()
                        {
                            TotalAulasDadas = aulasDadas,
                            TotalPresencas = 0,
                            TotalRemoto = 0,
                            TotalAusencias = 0,
                            TotalCompensacoes = 0,
                            TotalPercentualFrequencia = "",
                        }
                    }); ;
                    relatorio.Alunos.Add(relatorioFrequenciaIndividualAlunosDto);
                }
            }
        }

        private async Task<string> ObterSituacao(AlunoRetornoDto ultimaSituacaoAluno, bool ehInfantil, IEnumerable<PeriodoEscolar> periodosEscolares, Turma turma)
        {
            const int DiasDesdeInicioBimestreParaMarcadorNovo = 15;

            var periodoEscolar = periodosEscolares.SingleOrDefault(s => s.PeriodoFim >= ultimaSituacaoAluno.DataSituacao && s.PeriodoFim <= ultimaSituacaoAluno.DataSituacao);

            switch (ultimaSituacaoAluno.CodigoSituacaoMatricula)
            {
                case SituacaoMatriculaAluno.Ativo:
                    if ((ultimaSituacaoAluno.DataSituacao > periodoEscolar.PeriodoInicio) && (ultimaSituacaoAluno.DataSituacao.AddDays(DiasDesdeInicioBimestreParaMarcadorNovo) >= DateTime.Now.Date))
                        return $" - {(ehInfantil ? "Criança Nova" : "Estudante Novo")}: Data da matrícula {ultimaSituacaoAluno.DataSituacaoFormatada()}";
                    else
                        return string.Empty;

                case SituacaoMatriculaAluno.Transferido:
                    var alunoTransferido = await ObterAluno(ultimaSituacaoAluno, turma);
                    var detalheEscola = alunoTransferido.Transferencia_Interna ?
                                        $"para escola {alunoTransferido.EscolaTransferencia} e turma {alunoTransferido.TurmaTransferencia}" :
                                        "para outras redes";

                    return $" - {(ehInfantil ? "Criança Transferida" : "Estudante Transferido")}: {detalheEscola} em {ultimaSituacaoAluno.DataSituacaoFormatada()}";

                case SituacaoMatriculaAluno.RemanejadoSaida:
                    var alunoRemanejado = await ObterAluno(ultimaSituacaoAluno, turma);
                    return alunoRemanejado == null ?
                            string.Empty :
                            $" - {(ehInfantil ? "Criança Remanejada" : "Estudante Remanejado")}: turma {alunoRemanejado.TurmaRemanejamento} em {ultimaSituacaoAluno.DataSituacaoFormatada()}";

                case SituacaoMatriculaAluno.Desistente:
                case SituacaoMatriculaAluno.VinculoIndevido:
                case SituacaoMatriculaAluno.Falecido:
                case SituacaoMatriculaAluno.NaoCompareceu:
                case SituacaoMatriculaAluno.Deslocamento:
                case SituacaoMatriculaAluno.Cessado:
                case SituacaoMatriculaAluno.ReclassificadoSaida:
                    return $" - {(ehInfantil ? "Criança Inativa" : "Estudante Inativo")} em {ultimaSituacaoAluno.DataSituacaoFormatada()}";

                default:
                    return string.Empty;
            }
        }

        private async Task<AlunoPorTurmaRespostaDto> ObterAluno(AlunoRetornoDto ultimaSituacaoAluno, Turma turma)
        {
            var alunos = await mediator.Send(new ObterAlunosPorTurmaEDataMatriculaQuery(turma.Codigo, ultimaSituacaoAluno.DataSituacao));
            var aluno = alunos.OrderByDescending(o => o.DataSituacao).FirstOrDefault(s => s.CodigoAluno.Equals(ultimaSituacaoAluno.AlunoCodigo.ToString()));
            return aluno;
        }

        private async Task<DreUe> ObterNomeDreUe(string turmaCodigo)
            => await mediator.Send(new ObterDreUePorTurmaQuery(turmaCodigo));
    }
}
