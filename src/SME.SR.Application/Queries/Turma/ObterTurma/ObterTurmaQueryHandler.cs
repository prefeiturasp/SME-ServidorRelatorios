using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterTurmaQueryHandler : IRequestHandler<ObterTurmaQuery, Turma>
    {
        private ITurmaRepository _turmaSgpRepository;

        public ObterTurmaQueryHandler(ITurmaRepository turmaSgpRepository)
        {
            this._turmaSgpRepository = turmaSgpRepository;
        }

        public async Task<Turma> Handle(ObterTurmaQuery request, CancellationToken cancellationToken)
        {
            return await _turmaSgpRepository.ObterPorCodigo(request.CodigoTurma);
        }
    }
}
