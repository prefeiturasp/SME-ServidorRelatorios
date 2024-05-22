using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterObterProdutividadeFrequenciaPorFiltroQueryHandler : IRequestHandler<ObterObterProdutividadeFrequenciaPorFiltroQuery, IEnumerable<ConsolidacaoProdutividadeFrequenciaDto>>
    {
        private readonly IConsolidacaoProdutividadeFrequenciaRepository consolidadoRepository;

        public ObterObterProdutividadeFrequenciaPorFiltroQueryHandler(IConsolidacaoProdutividadeFrequenciaRepository consolidadoRepository)
        {
            this.consolidadoRepository = consolidadoRepository ?? throw new ArgumentNullException(nameof(consolidadoRepository));
        }

        public async Task<IEnumerable<ConsolidacaoProdutividadeFrequenciaDto>> Handle(ObterObterProdutividadeFrequenciaPorFiltroQuery request, CancellationToken cancellationToken)
            => AgruparPorTipoRelatorio(await consolidadoRepository.ObterConsolidacoesProdutividadeFrequenciaFiltro(request.Filtro), request.Filtro.TipoRelatorioProdutividade);   

        private IEnumerable<ConsolidacaoProdutividadeFrequenciaDto> AgruparPorTipoRelatorio(IEnumerable<ConsolidacaoProdutividadeFrequenciaDto> consolidacoes, TipoRelatorioProdutividadeFrequencia tipoRelatorio)
        {
            return tipoRelatorio switch
            {
                TipoRelatorioProdutividadeFrequencia.MédiaPorUE => consolidacoes.AgruparPorUe(),
                TipoRelatorioProdutividadeFrequencia.MédiaPorProfessor => consolidacoes.AgruparPorProfessor(),
                _ => consolidacoes,
            };
        }

    }
}
