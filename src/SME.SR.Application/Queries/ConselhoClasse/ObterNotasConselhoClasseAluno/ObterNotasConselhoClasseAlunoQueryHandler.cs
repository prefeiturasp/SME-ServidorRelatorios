using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterNotasConselhoClasseAlunoQueryHandler : IRequestHandler<ObterNotasConselhoClasseAlunoQuery, IEnumerable<NotaConceitoBimestreComponente>>
    {
        private readonly IConselhoClasseNotaRepository conselhoClasseNotaRepository;

        public ObterNotasConselhoClasseAlunoQueryHandler(IConselhoClasseNotaRepository conselhoClasseNotaRepository)
        {
            this.conselhoClasseNotaRepository = conselhoClasseNotaRepository ?? throw new ArgumentNullException(nameof(conselhoClasseNotaRepository));
        }

        public async Task<IEnumerable<NotaConceitoBimestreComponente>> Handle(ObterNotasConselhoClasseAlunoQuery request, CancellationToken cancellationToken)
        {
            return await conselhoClasseNotaRepository.ObterNotasAluno(request.ConselhoClasseId, request.CodigoAluno);
        }
    }
}
