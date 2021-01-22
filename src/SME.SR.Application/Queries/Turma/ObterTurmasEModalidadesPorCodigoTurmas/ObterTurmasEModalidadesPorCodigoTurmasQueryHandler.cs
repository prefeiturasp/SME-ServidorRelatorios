using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterTurmasEModalidadesPorCodigoTurmasQueryHandler : IRequestHandler<ObterTurmasEModalidadesPorCodigoTurmasQuery, IEnumerable<TurmaResumoDto>>
    {
        private readonly ITurmaRepository turmaRepository;

        public ObterTurmasEModalidadesPorCodigoTurmasQueryHandler(ITurmaRepository turmaRepository)
        {
            this.turmaRepository = turmaRepository ?? throw new ArgumentNullException(nameof(turmaRepository));
        }
        public async Task<IEnumerable<TurmaResumoDto>> Handle(ObterTurmasEModalidadesPorCodigoTurmasQuery request, CancellationToken cancellationToken)
        {
            return await turmaRepository.ObterTurmasResumoPorCodigos(request.TurmasCodigos);
        }
    }
}
