using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
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
        public ObterDadosRelatorioAcompanhamentoFrequenciaCommandHandler(IMediator mediator, IAlunoRepository alunoRepository)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this.alunoRepository = alunoRepository ?? throw new ArgumentNullException(nameof(alunoRepository));
        }
        public async Task<RelatorioFrequenciaIndividualDto> Handle(ObterDadosRelatorioAcompanhamentoFrequenciaCommand request, CancellationToken cancellationToken)
        {
            var relatorio = new RelatorioFrequenciaIndividualDto();
            var alunosSelecionados = new List<AlunoNomeDto>();
            var turma = await ObterDadosDaTurma(request.FiltroRelatorio.TurmaCodigo);
            await MapearCabecalho(relatorio, request.FiltroRelatorio, turma);

            var tipoCalendarioId = await mediator.Send(new ObterTipoCalendarioIdPorTurmaQuery(turma));
            if (tipoCalendarioId == default)
                throw new NegocioException("O tipo de calendário da turma não foi encontrado.");

            var periodosEscolares = await mediator.Send(new ObterPeriodosEscolaresPorTipoCalendarioQuery(tipoCalendarioId));

            int aulasDadas = await mediator.Send(new ObterAulasDadasNoBimestreQuery(turma.Codigo, tipoCalendarioId, long.Parse(request.FiltroRelatorio.ComponenteCurricularId), int.Parse(request.FiltroRelatorio.Bimestre)));

            relatorio.ehTodosBimestre = request.FiltroRelatorio.Bimestre.Equals("-99");
            if (request.FiltroRelatorio.AlunosCodigos.Contains("-99"))
            {
                var alunos = await mediator.Send(new ObterAlunosPorTurmaQuery() { TurmaCodigo = request.FiltroRelatorio.TurmaCodigo });
                if (alunos == null && !alunos.Any())
                    throw new NegocioException("Alunos não encontrados.");

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
                alunosSelecionados = (await alunoRepository.ObterNomesAlunosPorCodigos(request.FiltroRelatorio.AlunosCodigos.ToArray())).ToList();

            if (alunosSelecionados != null && alunosSelecionados.Any())
            {
                var dadosFrequencia = await mediator.Send(new ObterFrequenciaAlunoPorCodigoBimestreQuery(request.FiltroRelatorio.Bimestre, alunosSelecionados.Select(s => s.Codigo).ToArray(), request.FiltroRelatorio.TurmaCodigo, TipoFrequenciaAluno.PorDisciplina, request.FiltroRelatorio.ComponenteCurricularId));
                               
                var dadosAusencia = await mediator.Send(new ObterAusenciaPorAlunoTurmaBimestreQuery(alunosSelecionados.Select(s => s.Codigo).ToArray(), request.FiltroRelatorio.TurmaCodigo, request.FiltroRelatorio.Bimestre));
                await MapearAlunos(alunosSelecionados, relatorio, dadosFrequencia, dadosAusencia, turma, periodosEscolares, aulasDadas, int.Parse(request.FiltroRelatorio.Bimestre));
            }
            return relatorio;
        }

        private void MapearBimestre(IEnumerable<FrequenciaAlunoConsolidadoDto> dadosFrequenciaDto, IEnumerable<AusenciaBimestreDto> ausenciaBimestreDto, RelatorioFrequenciaIndividualAlunosDto aluno)
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

                        bimestre.DadosFrequencia = new RelatorioFrequenciaIndividualDadosFrequenciasDto
                        {
                            TotalAulasDadas = item.TotalAula,
                            TotalPresencas = item.TotalPresencas,
                            TotalRemoto = item.TotalRemotos,
                            TotalAusencias = item.TotalAusencias,
                            TotalCompensacoes = item.TotalCompensacoes,
                            TotalPercentualFrequencia = Math.Round(item.TotalPercentualFrequencia, 0).ToString(),
                            TotalPercentualFrequenciaFormatado = item.TotalPercentualFrequenciaFormatado
                        };

                        if (ausenciaBimestreDto != null && ausenciaBimestreDto.Any())
                        {
                            foreach (var ausencia in ausenciaBimestreDto)
                            {
                                if (item.CodigoAluno == ausencia.CodigoAluno && item.Bimestre == ausencia.Bimestre)
                                {
                                    bimestre.Justificativas.Add(new RelatorioFrequenciaIndividualJustificativasDto
                                    {
                                        DataAusencia = ausencia.DataAusencia.ToString("dd/MM/yyyy"),
                                        MotivoAusencia = UtilHtml.FormatarHtmlParaTexto(ausencia.MotivoAusencia),
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

        private async Task MapearCabecalho(RelatorioFrequenciaIndividualDto relatorio, FiltroAcompanhamentoFrequenciaJustificativaDto filtroRelatorio, Turma turma)
        {
            var dadosDreUe = await ObterNomeDreUe(filtroRelatorio.TurmaCodigo);

            relatorio.RF = filtroRelatorio.UsuarioRF;
            relatorio.Usuario = filtroRelatorio.UsuarioNome;
            relatorio.DreNome = dadosDreUe.DreNome;
            relatorio.UeNome = dadosDreUe.UeNome;
            relatorio.ComponenteNome = await ObterNomeComponente(filtroRelatorio.ComponenteCurricularId);
            relatorio.TurmaNome = turma.NomePorFiltroModalidade(null);
            relatorio.ehInfantil = turma != null && turma.ModalidadeCodigo == Modalidade.Infantil;
        }
        private async Task<Turma> ObterDadosDaTurma(string turmaCodigo)       
            => await mediator.Send(new ObterTurmaPorCodigoQuery(turmaCodigo));        

        private async Task<string> ObterNomeComponente(string componenteCodigo)
            => await mediator.Send(new ObterNomeComponenteCurricularPorIdQuery(Convert.ToInt64(componenteCodigo)));        

        private async Task MapearAlunos(IEnumerable<AlunoNomeDto> alunos, RelatorioFrequenciaIndividualDto relatorio, IEnumerable<FrequenciaAlunoConsolidadoDto> dadosFrequenciaDto, IEnumerable<AusenciaBimestreDto> ausenciaBimestreDto, Turma turma, IEnumerable<PeriodoEscolar> periodosEscolares, int aulasDadas, int bimestre)
        {
            foreach (var aluno in alunos.OrderBy(x => x.Nome))
            {
                var descricaoSituacaoAluno = string.Empty;

                var situacaoAluno = (await alunoRepository.ObterAlunosPorTurmaCodigoParaRelatorioAcompanhamentoAprendizagem(long.Parse(turma.Codigo), long.Parse(aluno.Codigo), turma.AnoLetivo));

                if (situacaoAluno != null && situacaoAluno.Any())
                {
                    var dataUltimaSituacao = situacaoAluno.Max(a => a.DataSituacao);

                    var ultimaSituacaoAluno = situacaoAluno.Where(x => x.DataSituacao == dataUltimaSituacao).FirstOrDefault();

                    var ehInfantil = turma != null && turma.ModalidadeCodigo == Modalidade.Infantil;

                    descricaoSituacaoAluno = ultimaSituacaoAluno.Ativo ? string.Empty : await ObterSituacao(ultimaSituacaoAluno, ehInfantil, periodosEscolares, turma);
                }
                var relatorioFrequenciaIndividualAlunosDto = new RelatorioFrequenciaIndividualAlunosDto
                {
                    NomeAluno = $"{aluno.Nome} ({aluno.Codigo}) {descricaoSituacaoAluno}",
                    CodigoAluno = aluno.Codigo
                };

                if (dadosFrequenciaDto != null && dadosFrequenciaDto.Where(x => x.CodigoAluno == aluno.Codigo).Any())
                {
                    if (relatorio != null)
                    {
                        MapearBimestre(dadosFrequenciaDto, ausenciaBimestreDto, relatorioFrequenciaIndividualAlunosDto);
                    }
                    relatorio.Alunos.Add(relatorioFrequenciaIndividualAlunosDto);
                }
                else
                {
                    var NomeBimestre = bimestre == 0 ? "Bimestre Final" : $"{bimestre}º Bimestre";
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
            var aluno = alunos.SingleOrDefault(s => s.CodigoAluno.Equals(ultimaSituacaoAluno.AlunoCodigo.ToString()));
            return aluno;
        }

        private async Task<DreUe> ObterNomeDreUe(string turmaCodigo)
            => await mediator.Send(new ObterDreUePorTurmaQuery(turmaCodigo));        
    }
}
