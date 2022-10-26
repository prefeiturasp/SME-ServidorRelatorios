using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterFechamentoConsolidadoPorTurmasQueryHandler : IRequestHandler<ObterFechamentoConsolidadoPorTurmasQuery, IEnumerable<FechamentoConsolidadoComponenteTurmaDto>>
    {
        private readonly IFechamentoConsolidadoRepository fechamentoConsolidadoRepository;

        public ObterFechamentoConsolidadoPorTurmasQueryHandler(IFechamentoConsolidadoRepository fechamentoConsolidadoRepository)
        {
            this.fechamentoConsolidadoRepository = fechamentoConsolidadoRepository ?? throw new System.ArgumentNullException(nameof(fechamentoConsolidadoRepository));
        }

        public async Task<IEnumerable<FechamentoConsolidadoComponenteTurmaDto>> Handle(ObterFechamentoConsolidadoPorTurmasQuery request, CancellationToken cancellationToken)
        {
            return await fechamentoConsolidadoRepository.ObterFechamentoConsolidadoPorTurmas(request.TurmasCodigo,request.Semestres,request.Bimestres);     
        }
    }
}
