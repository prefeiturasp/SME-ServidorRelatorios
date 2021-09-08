using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterFechamentoConsolidadoPorTurmasBimestreQueryHandler : IRequestHandler<ObterFechamentoConsolidadoPorTurmasBimestreQuery, IEnumerable<FechamentoConsolidadoComponenteTurmaDto>>
    {
        private readonly IFechamentoConsolidadoRepository fechamentoConsolidadoRepository;

        public ObterFechamentoConsolidadoPorTurmasBimestreQueryHandler(IFechamentoConsolidadoRepository fechamentoConsolidadoRepository)
        {
            this.fechamentoConsolidadoRepository = fechamentoConsolidadoRepository ?? throw new System.ArgumentNullException(nameof(fechamentoConsolidadoRepository));
        }

        public async Task<IEnumerable<FechamentoConsolidadoComponenteTurmaDto>> Handle(ObterFechamentoConsolidadoPorTurmasBimestreQuery request, CancellationToken cancellationToken)
        {
            return await fechamentoConsolidadoRepository.ObterFechamentoConsolidadoPorTurmasBimestre(request.TurmasCodigo,
                                                                                                     request.Bimestres,
                                                                                                     request.SituacaoFechamento);
        }
    }
}
