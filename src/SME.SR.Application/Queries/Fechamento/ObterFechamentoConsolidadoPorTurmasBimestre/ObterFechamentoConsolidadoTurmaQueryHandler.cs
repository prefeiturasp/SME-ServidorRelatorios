using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterFechamentoConsolidadoTurmaQueryHandler : IRequestHandler<ObterFechamentoConsolidadoTurmaQuery, IEnumerable<FechamentoConsolidadoTurmaDto>>
    {
        private readonly IFechamentoConsolidadoRepository fechamentoConsolidadoRepository;
        public ObterFechamentoConsolidadoTurmaQueryHandler(IFechamentoConsolidadoRepository fechamentoConsolidadoRepository)
        {
            this.fechamentoConsolidadoRepository = fechamentoConsolidadoRepository ?? throw new System.ArgumentNullException(nameof(fechamentoConsolidadoRepository));
        }

        public async Task<IEnumerable<FechamentoConsolidadoTurmaDto>> Handle(ObterFechamentoConsolidadoTurmaQuery request, CancellationToken cancellationToken)
        {
            return await fechamentoConsolidadoRepository.ObterFechamentoConsolidadoPorTurmasTodasUe(request.TurmasCodigo,request.ModalidadeCodigo);;
        }
    }
}
