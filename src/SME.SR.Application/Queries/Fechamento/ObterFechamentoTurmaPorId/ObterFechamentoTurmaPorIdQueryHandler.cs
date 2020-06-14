using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterFechamentoTurmaPorIdQueryHandler : IRequestHandler<ObterFechamentoTurmaPorIdQuery, FechamentoTurma>
    {
        private IFechamentoTurmaRepository _fechamentoTurmaRepository;

        public ObterFechamentoTurmaPorIdQueryHandler(IFechamentoTurmaRepository fechamentoTurmaRepository)
        {
            this._fechamentoTurmaRepository = fechamentoTurmaRepository;
        }

        public async Task<FechamentoTurma> Handle(ObterFechamentoTurmaPorIdQuery request, CancellationToken cancellationToken)
        {
            return await _fechamentoTurmaRepository.ObterTurmaPeriodoFechamentoPorId(request.FechamentoTurmaId);
        }
    }
}
