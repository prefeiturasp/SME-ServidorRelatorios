using MediatR;
using SME.SR.Data.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterParecerConclusivoPorAlunoQueryHandler : IRequestHandler<ObterParecerConclusivoPorAlunoQuery, string>
    {
        private readonly IConselhoClasseAlunoRepository conselhoClasseAlunoRepository;

        public ObterParecerConclusivoPorAlunoQueryHandler(IConselhoClasseAlunoRepository conselhoClasseAlunoRepository)
        {
            this.conselhoClasseAlunoRepository = conselhoClasseAlunoRepository ?? throw new ArgumentNullException(nameof(conselhoClasseAlunoRepository));
        }

        public async Task<string> Handle(ObterParecerConclusivoPorAlunoQuery request, CancellationToken cancellationToken)
        {
            return await conselhoClasseAlunoRepository.ObterParecerConclusivo(request.ConselhoClasseId, request.CodigoAluno);
        }
    }
}
