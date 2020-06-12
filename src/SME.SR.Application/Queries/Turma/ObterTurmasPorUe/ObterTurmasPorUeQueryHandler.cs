using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterTurmasPorUeQueryHandler : IRequestHandler<ObterTurmasPorUeQuery, IEnumerable<Turma>>
    {
        private ITurmaRepository _turmaSgpRepository;

        public ObterTurmasPorUeQueryHandler(ITurmaRepository turmaSgpRepository)
        {
            this._turmaSgpRepository = turmaSgpRepository;
        }

        public async Task<IEnumerable<Turma>> Handle(ObterTurmasPorUeQuery request, CancellationToken cancellationToken)
        {
            return await _turmaSgpRepository.ObterPorUe(request.CodigoUe, null, null, null); // TODO implementar filtros
        }
    }
}
