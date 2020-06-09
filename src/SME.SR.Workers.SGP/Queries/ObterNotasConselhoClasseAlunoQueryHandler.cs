using MediatR;
using SME.SR.Workers.SGP.Commons;
using SME.SR.Workers.SGP.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Workers.SGP.Queries
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
