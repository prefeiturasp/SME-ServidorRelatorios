using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterTurmasPorAbrangenciaFiltroQueryHandler : IRequestHandler<ObterTurmasPorAbrangenciaFiltroQuery, IEnumerable<Turma>>
    {
        private ITurmaRepository _turmaSgpRepository;

        public ObterTurmasPorAbrangenciaFiltroQueryHandler(ITurmaRepository turmaSgpRepository)
        {
            this._turmaSgpRepository = turmaSgpRepository;
        }

        public async Task<IEnumerable<Turma>> Handle(ObterTurmasPorAbrangenciaFiltroQuery request, CancellationToken cancellationToken)
        {
            return await _turmaSgpRepository.ObterPorAbrangenciaFiltros(request.CodigoUe, request.Modalidade, request.AnoLetivo, request.Login, request.Perfil, request.ConsideraHistorico, request.Semestre);
        }
    }
}
