using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using SME.SR.Infra.Utilitarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterRelatorioDeFrequenciaGlobalQueryHandler : IRequestHandler<ObterRelatorioDeFrequenciaGlobalQuery, List<FrequenciaGlobalDto>>
    {
        private const string FILTRO_OPCAO_TODOS = "-99";

        private readonly IFrequenciaAlunoRepository frequenciaAlunoRepository;
        private readonly IMediator mediator;

        public ObterRelatorioDeFrequenciaGlobalQueryHandler(IFrequenciaAlunoRepository frequenciaAlunoRepository, IMediator mediator)
        {
            this.frequenciaAlunoRepository = frequenciaAlunoRepository ?? throw new ArgumentNullException(nameof(frequenciaAlunoRepository));
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<List<FrequenciaGlobalDto>> Handle(ObterRelatorioDeFrequenciaGlobalQuery request, CancellationToken cancellationToken)
        {
            var retornoQuery = await frequenciaAlunoRepository.ObterFrequenciaAlunoMensal(request.Filtro.ExibirHistorico, request.Filtro.AnoLetivo,
                request.Filtro.CodigoDre, request.Filtro.CodigoUe, request.Filtro.Modalidade, request.Filtro.Semestre, request.Filtro.CodigosTurmas.Select(c => c).ToArray(),
                request.Filtro.MesesReferencias.Select(c => Convert.ToInt32(c)).ToArray(), request.Filtro.ApenasAlunosPercentualAbaixoDe);

            if (retornoQuery == null || !retornoQuery.Any())
                throw new NegocioException("Não há dados a serem impressos para o filtro selecionado.");

            return await MapearRetornoQuery(request.Filtro, retornoQuery);
        }

        private async Task<List<FrequenciaGlobalDto>> MapearRetornoQuery(FiltroFrequenciaGlobalDto filtro, IEnumerable<FrequenciaAlunoMensalConsolidadoDto> retornoQuery)
        {
            var retornoMapeado = new List<FrequenciaGlobalDto>();

            foreach (var agrupamentoUe in retornoQuery.GroupBy(x => x.UeCodigo))
            {
                var alunosEscola = await mediator
                    .Send(new ObterDadosAlunosEscolaQuery(agrupamentoUe.Key, filtro.CodigoDre, filtro.AnoLetivo, null));

                var agrupamento = alunosEscola.OrderBy(x => x.CodigoAluno).GroupBy(x => new { x.CodigoAluno, x.NomeAluno, x.CodigoTurma });

                foreach (var item in agrupamentoUe)
                {
                    var alunoAgrupado = agrupamento
                        .Where(c => c.Key.CodigoAluno.ToString() == item.CodigoEol && c.Key.CodigoTurma == item.TurmaCodigo);

                    var dadosSituacaoAluno = await DeveImprimirNoRelatorio(alunoAgrupado, item.Mes, filtro.AnoLetivo);

                    if (dadosSituacaoAluno.ImprimirRelatorio)
                    {
                        var aluno = alunosEscola.Select(c => new { c.CodigoAluno, c.NomeAluno, c.NomeSocialAluno, c.NumeroAlunoChamada, c.CodigoTurma })
                            .FirstOrDefault(c => c.CodigoAluno.ToString() == item.CodigoEol && (filtro.CodigosTurmas.First() == FILTRO_OPCAO_TODOS || (filtro.CodigosTurmas.First() != FILTRO_OPCAO_TODOS && filtro.CodigosTurmas.Contains(c.CodigoTurma))));

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
                            NumeroChamadda = aluno?.NumeroAlunoChamada ?? string.Empty,
                            PercentualFrequencia = item.Percentual
                        });
                    }
                }
            }

            var retornoOrdenado = retornoMapeado.OrderBy(c => c.SiglaDre)
                .ThenBy(c => c.UeNome)
                .ThenBy(c => c.Mes)
                .ThenBy(c => c.Turma)
                .ThenBy(c => c.Estudante);

            return retornoOrdenado.ToList();
        }

        private async Task<UltimaSituacaoAlunoRelatorioFrequenciaGlobalDto> DeveImprimirNoRelatorio(IEnumerable<IGrouping<object, DadosAlunosEscolaDto>> agrupamento, int mesSelecionado, int anoLetivoSelecionado)
        {
            var dadosSituacaoAluno = new UltimaSituacaoAlunoRelatorioFrequenciaGlobalDto { ImprimirRelatorio = false };
            var itemsMesSelecionado = await FiltrarMatriculasConsideradasNoMes(agrupamento, mesSelecionado, anoLetivoSelecionado);

            if (itemsMesSelecionado.Any())
            {
                var ultimoRegistroMatricula = itemsMesSelecionado.Last().Last();
                var inativoNoMes = itemsMesSelecionado
                    .Any(i => !SituacoesAtiva.Contains(i.Last().CodigoSituacaoMatricula) && i.Last().DataSituacao.Month == mesSelecionado);

                dadosSituacaoAluno.ImprimirRelatorio = true;
                dadosSituacaoAluno.NomeFinalAluno = ultimoRegistroMatricula.ObterNomeFinal();

                if (inativoNoMes)
                    dadosSituacaoAluno.NomeFinalAluno += " - " + ultimoRegistroMatricula.SituacaoMatricula
                        + " " + ultimoRegistroMatricula.DataSituacao.ToString("dd/MM/yyyy");
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

        private async Task<IEnumerable<IGrouping<object, DadosAlunosEscolaDto>>> FiltrarMatriculasConsideradasNoMes(IEnumerable<IGrouping<object, DadosAlunosEscolaDto>> lista, int mesConsiderado, int anoLetivoSelecionado)
        {
            if (lista.Any(l => l.First().DataMatricula.Year == anoLetivoSelecionado || l.First().DataSituacao.Year == anoLetivoSelecionado))
                return await Task.FromResult(lista
                     .Where(l => (SituacoesAtiva.Contains(l.Last().CodigoSituacaoMatricula) && (l.First().DataMatricula.Month < mesConsiderado || (l.First().DataSituacao.Month == mesConsiderado && l.First().DataSituacao.Day < DateTime.DaysInMonth(l.First().AnoLetivo, mesConsiderado))) ||
                                 (!SituacoesAtiva.Contains(l.Last().CodigoSituacaoMatricula) && l.Last().DataSituacao.Month >= mesConsiderado))));
            else
                return await Task.FromResult(lista.Where(l => (SituacoesAtiva.Contains(l.Last().CodigoSituacaoMatricula))));
        }
    }
}
