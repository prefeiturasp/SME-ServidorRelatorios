using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SME.SR.Data;
using SME.SR.Infra.Dtos.AE.Adesao;

namespace SME.SR.Application
{
    internal class ObterValoresConsolidadosAdesaoAppQueryHandler : IRequestHandler<ObterValoresConsolidadosAdesaoAppQuery, IEnumerable<AdesaoAEQueryConsolidadoRetornoDto>>
    {
        private readonly IDashboardAdesaoRepository dashboardAdesaoRepository;

        public ObterValoresConsolidadosAdesaoAppQueryHandler(IDashboardAdesaoRepository dashboardAdesaoRepository)
        {
            this.dashboardAdesaoRepository = dashboardAdesaoRepository ?? throw new System.ArgumentNullException(nameof(dashboardAdesaoRepository));
        }
        public async Task<IEnumerable<AdesaoAEQueryConsolidadoRetornoDto>> Handle(ObterValoresConsolidadosAdesaoAppQuery request, CancellationToken cancellationToken)
        {
            return await dashboardAdesaoRepository.ObterAdesaoDashboardPorFiltros(request.DreCodigo, request.UeCodigo);
        }
    }
}