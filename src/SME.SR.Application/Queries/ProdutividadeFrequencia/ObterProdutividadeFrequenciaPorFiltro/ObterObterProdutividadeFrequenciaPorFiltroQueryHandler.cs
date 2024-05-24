using MediatR;
using Nest;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterObterProdutividadeFrequenciaPorFiltroQueryHandler : IRequestHandler<ObterObterProdutividadeFrequenciaPorFiltroQuery, IEnumerable<ConsolidacaoProdutividadeFrequenciaDto>>
    {
        private readonly IConsolidacaoProdutividadeFrequenciaRepository consolidadoRepository;
        private readonly IMediator mediator;

        public ObterObterProdutividadeFrequenciaPorFiltroQueryHandler(IConsolidacaoProdutividadeFrequenciaRepository consolidadoRepository, IMediator mediator)
        {
            this.consolidadoRepository = consolidadoRepository ?? throw new ArgumentNullException(nameof(consolidadoRepository));
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator)); ;
        }

        public async Task<IEnumerable<ConsolidacaoProdutividadeFrequenciaDto>> Handle(ObterObterProdutividadeFrequenciaPorFiltroQuery request, CancellationToken cancellationToken)
        {
            var consolidacoes = (await consolidadoRepository.ObterConsolidacoesProdutividadeFrequenciaFiltro(request.Filtro)).ToList();
            if (request.Filtro.TipoRelatorioProdutividade == TipoRelatorioProdutividadeFrequencia.Analitico)
                await PreencherNomesComponentes(consolidacoes);
            return AgruparPorTipoRelatorio(consolidacoes, request.Filtro.TipoRelatorioProdutividade);
        }

        private async Task<IEnumerable<(string codigoComponente, string nomeComponente)>> ObterNomesComponentesTurma(string codigoTurma, long[] codigosComponente)
        {
            var componentes = await mediator.Send(new ObterComponentesCurricularesEolPorIdsQuery(codigosComponente, new string[] {codigoTurma}));
            return componentes.Select(c => (c.CodDisciplina.ToString(), c.Disciplina));
        }

        private IEnumerable<ConsolidacaoProdutividadeFrequenciaDto> AgruparPorTipoRelatorio(IEnumerable<ConsolidacaoProdutividadeFrequenciaDto> consolidacoes, TipoRelatorioProdutividadeFrequencia tipoRelatorio)
        {
            return tipoRelatorio switch
            {
                TipoRelatorioProdutividadeFrequencia.MédiaPorUE => consolidacoes.AgruparPorUe(),
                TipoRelatorioProdutividadeFrequencia.MédiaPorProfessor => consolidacoes.AgruparPorProfessor(),
                _ => consolidacoes,
            };
        }

        private async Task PreencherNomesComponentes(List<ConsolidacaoProdutividadeFrequenciaDto> consolidacoes)
        {
            var nomesComponentesTerritorio = await ObterNomesComponentes(consolidacoes);
            foreach (var consolidacao in consolidacoes.Where(c => string.IsNullOrEmpty(c.NomeComponenteCurricular)))
            {
                var componente = nomesComponentesTerritorio.FirstOrDefault(nmc => nmc.codigoTurma.Equals(consolidacao.CodigoTurma)
                                                                 && nmc.codigoComponente.Equals(consolidacao.CodigoComponenteCurricular));
                consolidacao.NomeComponenteCurricular = componente.nomeComponente;
            }
        }

        private async Task<List<(string codigoTurma, string codigoComponente, string nomeComponente)>> ObterNomesComponentes(IEnumerable<ConsolidacaoProdutividadeFrequenciaDto> consolidacoes)
        {
            var nomesComponentesTerritorio = new List<(string codigoTurma, string codigoComponente, string nomeComponente)>();
            foreach (var consolidacao in consolidacoes.Where(c => string.IsNullOrEmpty(c.NomeComponenteCurricular)).GroupBy(c => new { c.CodigoTurma }))
            {
                var nomesComponentes = await ObterNomesComponentesTurma(consolidacao.Key.CodigoTurma, consolidacao.Select(c => long.Parse(c.CodigoComponenteCurricular)).Distinct().ToArray());
                nomesComponentesTerritorio.AddRange(nomesComponentes.Select(c => (consolidacao.Key.CodigoTurma, c.codigoComponente, c.nomeComponente)));
            }
            return nomesComponentesTerritorio;
        }

    }
}
