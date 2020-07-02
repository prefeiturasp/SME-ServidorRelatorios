using MediatR;
using SME.SR.Data.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterConselhoClassePorFechamentoTurmaIdQueryHandler : IRequestHandler<ObterConselhoClassePorFechamentoTurmaIdQuery, long>
    {
        private readonly IConselhoClasseRepository conselhoClasseRepository;

        public ObterConselhoClassePorFechamentoTurmaIdQueryHandler(IConselhoClasseRepository conselhoClasseRepository)
        {
            this.conselhoClasseRepository = conselhoClasseRepository ?? throw new ArgumentNullException(nameof(turmaRepository));
        }

        public async Task<long> Handle(ObterConselhoClassePorFechamentoTurmaIdQuery request, CancellationToken cancellationToken)
        {
            return await conselhoClasseRepository.ObterConselhoPorFechamentoTurmaId(request.FechamentoTurmaId);
        }
    }
}
