using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterTurmasPorAbrangenciaFiltroQueryHandler : IRequestHandler<ObterTurmasPorAbrangenciaFiltroQuery, IEnumerable<Turma>>
    {
        private readonly ITurmaRepository turmaSgpRepository;

        public ObterTurmasPorAbrangenciaFiltroQueryHandler(ITurmaRepository turmaSgpRepository)
        {
            this.turmaSgpRepository = turmaSgpRepository ?? throw new ArgumentNullException(nameof(turmaSgpRepository));
        }

        public async Task<IEnumerable<Turma>> Handle(ObterTurmasPorAbrangenciaFiltroQuery request, CancellationToken cancellationToken)
        {
            var turmas = await turmaSgpRepository.ObterPorAbrangenciaFiltros(request.CodigoUe, request.Modalidade, request.AnoLetivo, request.Login, request.Perfil, request.ConsideraHistorico, request.Semestre, request.PossuiFechamento, request.SomenteEscolarizadas, request.CodigoDre);

            if (turmas == null || !turmas.Any())
                throw new NegocioException("Não foi possível encontrar as turmas da abrangência.");

            return turmas;
        }
    }
}
