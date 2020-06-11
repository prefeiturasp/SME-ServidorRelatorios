using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterNotasConselhoClasseAlunoQueryHandler : IRequestHandler<ObterNotasConselhoClasseAlunoQuery, IEnumerable<NotaConceitoBimestreComponente>>
    {
        private IConselhoClasseNotaRepository _conselhoClasseNotaRepository;

        public ObterNotasConselhoClasseAlunoQueryHandler(IConselhoClasseNotaRepository conselhoClasseNotaRepository)
        {
            this._conselhoClasseNotaRepository = conselhoClasseNotaRepository;
        }

        public async Task<IEnumerable<NotaConceitoBimestreComponente>> Handle(ObterNotasConselhoClasseAlunoQuery request, CancellationToken cancellationToken)
        {
            return await _conselhoClasseNotaRepository.ObterNotasAluno(request.ConselhoClasseId, request.CodigoAluno);
        }
    }
}
