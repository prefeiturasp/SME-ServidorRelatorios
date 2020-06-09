using MediatR;
using SME.SR.Workers.SGP.Commons;
using SME.SR.Workers.SGP.Models;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Workers.SGP.Queries
{
    public class ObterTurmaPeriodoFechamentoPorIdQueryHandler : IRequestHandler<ObterTurmaPeriodoFechamentoPorIdQuery, FechamentoTurma>
    {
        private IFechamentoTurmaRepository _fechamentoTurmaRepository;

        public ObterTurmaPeriodoFechamentoPorIdQueryHandler(IFechamentoTurmaRepository fechamentoTurmaRepository)
        {
            this._fechamentoTurmaRepository = fechamentoTurmaRepository;
        }

        public async Task<FechamentoTurma> Handle(ObterTurmaPeriodoFechamentoPorIdQuery request, CancellationToken cancellationToken)
        {
            return await _fechamentoTurmaRepository.ObterTurmaPeriodoFechamentoPorId(request.FechamentoTurmaId);
        }
    }
}
