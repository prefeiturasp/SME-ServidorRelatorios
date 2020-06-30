using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterTurmasPorFiltroQueryHandler : IRequestHandler<ObterTurmasPorFiltroQuery, IEnumerable<Turma>>
    {
        private ITurmaRepository _turmaSgpRepository;

        public ObterTurmasPorFiltroQueryHandler(ITurmaRepository turmaSgpRepository)
        {
            this._turmaSgpRepository = turmaSgpRepository;
        }

        public async Task<IEnumerable<Turma>> Handle(ObterTurmasPorFiltroQuery request, CancellationToken cancellationToken)
        {
            return await _turmaSgpRepository.ObterPorFiltros(request.Usuario.Login, request.Usuario.PerfilAtual, request.CodigoUe, request.Modalidade, request.AnoLetivo, request.Semestre); 
        }
    }
}
