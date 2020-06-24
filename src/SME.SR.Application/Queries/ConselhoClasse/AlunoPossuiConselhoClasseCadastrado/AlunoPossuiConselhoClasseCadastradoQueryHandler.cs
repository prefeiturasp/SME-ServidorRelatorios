using MediatR;
using SME.SR.Data.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class AlunoPossuiConselhoClasseCadastradoQueryHandler : IRequestHandler<AlunoPossuiConselhoClasseCadastradoQuery, bool>
    {
        private IConselhoClasseAlunoRepository _conselhoClasseAlunoRepository;

        public AlunoPossuiConselhoClasseCadastradoQueryHandler(IConselhoClasseAlunoRepository conselhoClasseAlunoRepository)
        {
            this._conselhoClasseAlunoRepository = conselhoClasseAlunoRepository;
        }

        public async Task<bool> Handle(AlunoPossuiConselhoClasseCadastradoQuery request, CancellationToken cancellationToken)
        {
            return await _conselhoClasseAlunoRepository.PossuiConselhoClasseCadastrado(request.ConselhoClasseId, request.CodigoAluno);
        }
    }
}
