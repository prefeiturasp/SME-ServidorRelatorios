using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterFechamentoTurmaPorIdQueryHandler : IRequestHandler<ObterFechamentoTurmaPorIdQuery, FechamentoTurma>
    {
        private readonly IFechamentoTurmaRepository fechamentoTurmaRepository;

        public ObterFechamentoTurmaPorIdQueryHandler(IFechamentoTurmaRepository fechamentoTurmaRepository)
        {
            this.fechamentoTurmaRepository = fechamentoTurmaRepository ?? throw new ArgumentNullException(nameof(fechamentoTurmaRepository));
        }

        public async Task<FechamentoTurma> Handle(ObterFechamentoTurmaPorIdQuery request, CancellationToken cancellationToken)
        {
            return await fechamentoTurmaRepository.ObterTurmaPeriodoFechamentoPorId(request.FechamentoTurmaId);
        }
    }
}
