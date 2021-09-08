using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterPendenciasParaFechamentoConsolidadoQueryHandler : IRequestHandler<ObterPendenciasParaFechamentoConsolidadoQuery, IEnumerable<PendenciaParaFechamentoConsolidadoDto>>
    {
        private readonly IPendenciaFechamentoRepository pendenciaFechamentoRepository;

        public ObterPendenciasParaFechamentoConsolidadoQueryHandler(IPendenciaFechamentoRepository pendenciaFechamentoRepository)
        {
            this.pendenciaFechamentoRepository = pendenciaFechamentoRepository ?? throw new System.ArgumentNullException(nameof(pendenciaFechamentoRepository));
        }

        public async Task<IEnumerable<PendenciaParaFechamentoConsolidadoDto>> Handle(ObterPendenciasParaFechamentoConsolidadoQuery request, CancellationToken cancellationToken)
        {
            var pendenciasFechamento = await pendenciaFechamentoRepository.ObterPendenciasParaFechamentoConsolidado(request.TurmasCodigo, request.Bimestres, request.ComponentesCurricularesId);
            return pendenciasFechamento;
        }
    }
}
