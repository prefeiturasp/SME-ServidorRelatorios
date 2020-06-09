using MediatR;
using SME.SR.Workers.SGP.Commons;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Workers.SGP.Queries
{
    public class ObterParecerConclusivoPorAlunoQueryHandler : IRequestHandler<ObterParecerConclusivoPorAlunoQuery, string>
    {
        private IConselhoClasseAlunoRepository _conselhoClasseAlunoRepository;

        public ObterParecerConclusivoPorAlunoQueryHandler(IConselhoClasseAlunoRepository conselhoClasseAlunoRepository)
        {
            this._conselhoClasseAlunoRepository = conselhoClasseAlunoRepository;
        }

        public async Task<string> Handle(ObterParecerConclusivoPorAlunoQuery request, CancellationToken cancellationToken)
        {
            return await _conselhoClasseAlunoRepository.ObterParecerConclusivo(request.ConselhoClasseId, request.CodigoAluno);
        }
    }
}
