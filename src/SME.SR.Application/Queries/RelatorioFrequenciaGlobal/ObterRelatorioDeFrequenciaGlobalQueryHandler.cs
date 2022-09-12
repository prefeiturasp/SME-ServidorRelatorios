using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using SME.SR.Infra.Utilitarios;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterRelatorioDeFrequenciaGlobalQueryHandler : IRequestHandler<ObterRelatorioDeFrequenciaGlobalQuery, List<FrequenciaGlobalDto>>
    {
        private readonly IFrequenciaAlunoRepository frequenciaAlunoRepository;
        private readonly IMediator mediator;

        public ObterRelatorioDeFrequenciaGlobalQueryHandler(IFrequenciaAlunoRepository frequenciaAlunoRepository,
            IMediator mediator)
        {
            this.frequenciaAlunoRepository = frequenciaAlunoRepository ?? throw new ArgumentNullException(nameof(frequenciaAlunoRepository));
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<List<FrequenciaGlobalDto>> Handle(ObterRelatorioDeFrequenciaGlobalQuery request, CancellationToken cancellationToken)
        {
            var retornoQuery = await frequenciaAlunoRepository.ObterFrequenciaAlunoMensal(request.Filtro.ExibirHistorico, request.Filtro.AnoLetivo,
                request.Filtro.CodigoDre, request.Filtro.CodigoUe, request.Filtro.Modalidade, request.Filtro.Semestre, request.Filtro.CodigosTurmas.Select(c => c).ToArray(),
                request.Filtro.MesesReferencias.Select(c => Convert.ToInt32(c)).ToArray(), request.Filtro.ApenasAlunosPercentualAbaixoDe);

            if (retornoQuery == null || (retornoQuery.Count() == 0))
                return await MapearRetornoVazio(request.Filtro);

            return await MapearRetornoQuery(request.Filtro, retornoQuery);
        }

        private async Task<List<FrequenciaGlobalDto>> MapearRetornoVazio(FiltroFrequenciaGlobalDto filtro)
        {
            var retornoMapeado = new List<FrequenciaGlobalDto>();

            List<Dre> dres;
            List<Turma> turmas;

            if (string.IsNullOrEmpty(filtro.CodigoDre) || (filtro.CodigoDre == "-99"))
                dres = (await mediator.Send(new ObterTodasDresQuery())).ToList();
            else
                dres = new List<Dre> { await mediator.Send(new ObterDrePorCodigoQuery(filtro.CodigoDre)) };

            var ues = (await mediator.Send(new ObterPorDresIdQuery(dres.Select(c => c.Id).ToArray()))).ToList();

            if (!string.IsNullOrEmpty(filtro.CodigoUe) && (filtro.CodigoUe != "-99"))
                ues = ues.Where(c => c.Codigo == filtro.CodigoUe).ToList();

            var meses = filtro.MesesReferencias;

            if ((meses.Count == 0) || meses.Contains("-99"))
                meses = new List<string> { "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12" };

            foreach (var dre in dres)
            {
                foreach (var ue in ues)
                {
                    if ((filtro.CodigosTurmas.Count() == 0) || filtro.CodigosTurmas.Contains("-99"))
                        turmas = (await mediator.Send(new ObterTurmasPorUeEAnoLetivoQuery(ue.Codigo, filtro.AnoLetivo))).ToList();
                    else
                    {
                        turmas = (await mediator.Send(new ObterTurmasPorCodigoQuery(filtro.CodigosTurmas.Select(c => c).ToArray()))).ToList();

                        //-> Foi necessário fazer isso, pois, a query acima retorna no campo código.
                        turmas.ForEach(c => c.turma_id = c.Codigo);
                    }

                    var alunos = await mediator.Send(new ObterAlunosPorTurmasQuery(turmas.Select(c => long.Parse(c.turma_id))));

                    var dadosAlunosEscolas = await mediator.Send(new ObterDadosAlunosEscolaQuery(ue.Codigo, filtro.AnoLetivo,
                        alunos.Select(c => c.CodigoAluno.ToString()).ToArray()));

                    var agrupamento = dadosAlunosEscolas.Where(x => filtro.CodigosTurmas.ToList().Contains(x.CodigoTurma.ToString())).ToList().OrderBy(x => x.CodigoAluno)
                        .GroupBy(x => new { x.CodigoAluno, x.NomeAluno, x.CodigoTurma });

                    foreach (var turma in turmas)
                    {
                        foreach (var mes in meses)
                        {
                            foreach (var item in agrupamento.SelectMany(c => c))
                            {                                
                                var alunoDados = dadosAlunosEscolas.Select(c => new { c.CodigoAluno, c.NomeAluno, c.NomeSocialAluno, c.NumeroAlunoChamada, c.CodigoTurma })
                                    .FirstOrDefault(c => c.CodigoAluno == item.CodigoAluno);

                                var estaAtivo = SituacoesAtiva.Contains(item.CodigoSituacaoMatricula);

                                if (estaAtivo)
                                {
                                    retornoMapeado.Add(new FrequenciaGlobalDto()
                                    {
                                        SiglaDre = dre.Abreviacao,
                                        DreCodigo = dre.Codigo,
                                        UeNome = string.Concat(ue.TipoEscola.GetAttribute<DisplayAttribute>().ShortName, " - ", ue.Nome),
                                        UeCodigo = ue.Codigo,
                                        Mes = int.Parse(mes),
                                        TurmaCodigo = turma.Codigo,
                                        Turma = string.Concat(turma.ModalidadeCodigo.GetAttribute<DisplayAttribute>().ShortName, " - ", turma.Nome),
                                        CodigoEOL = alunoDados.CodigoAluno.ToString(),
                                        Estudante = string.IsNullOrEmpty(alunoDados.NomeSocialAluno) ? alunoDados.NomeAluno : alunoDados.NomeSocialAluno,
                                        NumeroChamadda = alunoDados.NumeroAlunoChamada,
                                        PercentualFrequencia = null
                                    });
                                }
                            }
                        }

                    }
                }
            }
            var retornoOrdenado = retornoMapeado
                .OrderBy(c => c.SiglaDre)
                .ThenBy(c => c.UeNome)
                .ThenBy(c => c.Mes)
                .ThenBy(c => c.Turma)
                .ThenBy(c => c.Estudante);

            return retornoOrdenado.ToList();
        }

        private async Task<List<FrequenciaGlobalDto>> MapearRetornoQuery(FiltroFrequenciaGlobalDto filtro,
            IEnumerable<FrequenciaAlunoMensalConsolidadoDto> retornoQuery)
        {
            var retornoMapeado = new List<FrequenciaGlobalDto>();
            var alunosEscola = await mediator.Send(new ObterDadosAlunosEscolaQuery(filtro.CodigoUe, filtro.AnoLetivo, retornoQuery.Select(c => c.CodigoEol).ToArray()));

            var agrupamento = alunosEscola.OrderBy(x => x.CodigoAluno).GroupBy(x => new { x.CodigoAluno, x.NomeAluno, x.CodigoTurma });

            foreach (var item in retornoQuery)
            {
                var alunoAgrupado = agrupamento.Where(c => c.FirstOrDefault().CodigoAluno.ToString() == item.CodigoEol);
                var dadosSituacaoAluno = DeveImprimirNoRelatorio(alunoAgrupado, item.Mes);

                if (dadosSituacaoAluno.ImprimirRelatorio)
                {
                    var aluno = alunosEscola.Select(c => new { c.CodigoAluno, c.NomeAluno, c.NomeSocialAluno, c.NumeroAlunoChamada, c.CodigoTurma })
                        .FirstOrDefault(c => c.CodigoAluno.ToString() == item.CodigoEol);

                    retornoMapeado.Add(new FrequenciaGlobalDto()
                    {
                        SiglaDre = item.DreSigla,
                        DreCodigo = item.DreCodigo,
                        UeNome = string.Concat(item.DescricaoTipoEscola, " - ", item.UeNome),
                        UeCodigo = item.UeCodigo,
                        Mes = item.Mes,
                        TurmaCodigo = item.TurmaCodigo,
                        Turma = string.Concat(ObterModalidade(item.ModalidadeCodigo).ShortName(), " - ", item.TurmaNome),
                        CodigoEOL = item.CodigoEol,
                        Estudante = dadosSituacaoAluno.NomeFinalAluno,
                        NumeroChamadda = aluno.NumeroAlunoChamada,
                        PercentualFrequencia = item.Percentual
                    });
                }
            }

            var retornoOrdenado = retornoMapeado.OrderBy(c => c.SiglaDre)
                .ThenBy(c => c.UeNome)
                .ThenBy(c => c.Mes)
                .ThenBy(c => c.Turma)
                .ThenBy(c => c.Estudante);

            return retornoOrdenado.ToList();
        }

        private UltimaSituacaoAlunoRelatorioFrequenciaGlobalDto DeveImprimirNoRelatorio(IEnumerable<IGrouping<object, DadosAlunosEscolaDto>> agrupamento, int mesSelecionado)
        {
            var dadosSituacaoAluno = new UltimaSituacaoAlunoRelatorioFrequenciaGlobalDto
            {
                ImprimirRelatorio = false
            };

            bool ultimoStatusDoMesSelecionadoEhAtivo = false;
            var itemsMesSelecionado = agrupamento.Where(x => x.FirstOrDefault().DataSituacao.Month == mesSelecionado);

            if (itemsMesSelecionado?.Count() > 0)
            {
                if (itemsMesSelecionado?.FirstOrDefault()?.FirstOrDefault().DataSituacao.Day == 1)
                {
                    var ativoDiaUmDoMesSelecionado = SituacoesAtiva.Contains(itemsMesSelecionado.FirstOrDefault().FirstOrDefault().CodigoSituacaoMatricula);
                    ultimoStatusDoMesSelecionadoEhAtivo = SituacoesAtiva.Contains(itemsMesSelecionado.LastOrDefault().LastOrDefault().CodigoSituacaoMatricula);
                    if (ativoDiaUmDoMesSelecionado && !ultimoStatusDoMesSelecionadoEhAtivo)
                    {
                        dadosSituacaoAluno.ImprimirRelatorio = true;
                        dadosSituacaoAluno.NomeFinalAluno = itemsMesSelecionado.LastOrDefault()?.LastOrDefault()?.ObterNomeFinal() + " - " + itemsMesSelecionado.LastOrDefault()?.LastOrDefault()?.SituacaoMatricula
                                                                                                + " " + itemsMesSelecionado.LastOrDefault()?.LastOrDefault()?.DataSituacao.ToString("dd/MM/yyyy");
                    }
                    else if (ativoDiaUmDoMesSelecionado && ultimoStatusDoMesSelecionadoEhAtivo)
                    {
                        dadosSituacaoAluno.NomeFinalAluno = itemsMesSelecionado.FirstOrDefault()?.FirstOrDefault()?.ObterNomeFinal();
                        dadosSituacaoAluno.ImprimirRelatorio = true;
                    }
                    else if (!ativoDiaUmDoMesSelecionado)
                    {
                        dadosSituacaoAluno.NomeFinalAluno = itemsMesSelecionado.FirstOrDefault()?.FirstOrDefault()?.ObterNomeFinal();
                        dadosSituacaoAluno.ImprimirRelatorio = false;
                    }
                }
                else if (itemsMesSelecionado.FirstOrDefault()?.FirstOrDefault()?.DataSituacao.Day > 1)
                {
                    var ativoUltimoDiaMesAnterior = SituacoesAtiva.Contains(agrupamento?.ToList()?.LastOrDefault()?.Where(x => x.DataSituacao.Month < mesSelecionado)?.LastOrDefault()?.CodigoSituacaoMatricula
                                                                                                                                ?? itemsMesSelecionado.FirstOrDefault().FirstOrDefault().CodigoSituacaoMatricula);
                    ultimoStatusDoMesSelecionadoEhAtivo = SituacoesAtiva.Contains(itemsMesSelecionado.LastOrDefault().LastOrDefault().CodigoSituacaoMatricula);
                    if (ativoUltimoDiaMesAnterior && !ultimoStatusDoMesSelecionadoEhAtivo)
                    {
                        dadosSituacaoAluno.ImprimirRelatorio = true;
                        dadosSituacaoAluno.NomeFinalAluno = itemsMesSelecionado.LastOrDefault()?.LastOrDefault()?.ObterNomeFinal() + " - " + itemsMesSelecionado.LastOrDefault()?.LastOrDefault()?.SituacaoMatricula
                                                                                                + " " + itemsMesSelecionado.LastOrDefault()?.LastOrDefault()?.DataSituacao.ToString("dd/MM/yyyy");
                    }
                    else if (ativoUltimoDiaMesAnterior && ultimoStatusDoMesSelecionadoEhAtivo)
                    {
                        dadosSituacaoAluno.NomeFinalAluno = itemsMesSelecionado.FirstOrDefault()?.FirstOrDefault()?.ObterNomeFinal();
                        dadosSituacaoAluno.ImprimirRelatorio = true;
                    }
                    else if (itemsMesSelecionado.FirstOrDefault().FirstOrDefault().DataSituacao.Month >= mesSelecionado) 
                    {
                        dadosSituacaoAluno.NomeFinalAluno = itemsMesSelecionado.FirstOrDefault()?.FirstOrDefault()?.ObterNomeFinal();
                        dadosSituacaoAluno.ImprimirRelatorio = true;
                    }
                    else if (!ativoUltimoDiaMesAnterior)
                    {
                        dadosSituacaoAluno.NomeFinalAluno = itemsMesSelecionado.FirstOrDefault()?.FirstOrDefault()?.ObterNomeFinal();
                        dadosSituacaoAluno.ImprimirRelatorio = false;
                    }
                }
            }
            else
            {
                var estaAtivo = SituacoesAtiva.Contains((agrupamento?.FirstOrDefault()?.FirstOrDefault()?.CodigoSituacaoMatricula).GetValueOrDefault());
                dadosSituacaoAluno.ImprimirRelatorio = estaAtivo;
                dadosSituacaoAluno.NomeFinalAluno = estaAtivo ? agrupamento.FirstOrDefault()?.FirstOrDefault().ObterNomeFinal() : agrupamento.LastOrDefault()?.LastOrDefault()?.ObterNomeFinal() + " - " 
                                                       + agrupamento.LastOrDefault()?.LastOrDefault()?.SituacaoMatricula
                                                       + " " + agrupamento.LastOrDefault()?.LastOrDefault()?.DataSituacao.ToString("dd/MM/yyyy");
            }

            return dadosSituacaoAluno;
        }
        private int[] SituacoesAtiva => new[] { (int)SituacaoMatriculaAluno.Ativo,
                        (int)SituacaoMatriculaAluno.Rematriculado,
                        (int)SituacaoMatriculaAluno.PendenteRematricula,
                        (int)SituacaoMatriculaAluno.SemContinuidade,
                        (int)SituacaoMatriculaAluno.Concluido};

        private Modalidade ObterModalidade(int modalidadeCodigo)
        {
            return Enum.GetValues(typeof(Modalidade))
                .Cast<Modalidade>().FirstOrDefault(x => (int)x == modalidadeCodigo);
        }
    }
}
