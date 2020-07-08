using MediatR;
using SME.SR.Data.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class AlunoPossuiConselhoClasseCadastradoQueryHandler : IRequestHandler<AlunoPossuiConselhoClasseCadastradoQuery, bool>
    {
        private readonly IConselhoClasseAlunoRepository conselhoClasseAlunoRepository;

        public AlunoPossuiConselhoClasseCadastradoQueryHandler(IConselhoClasseAlunoRepository conselhoClasseAlunoRepository)
        {
            this.conselhoClasseAlunoRepository = conselhoClasseAlunoRepository ?? throw new ArgumentNullException(nameof(conselhoClasseAlunoRepository));
        }

        public async Task<bool> Handle(AlunoPossuiConselhoClasseCadastradoQuery request, CancellationToken cancellationToken)
        {
            return await conselhoClasseAlunoRepository.PossuiConselhoClasseCadastrado(request.ConselhoClasseId, request.CodigoAluno);
        }
    }
}
