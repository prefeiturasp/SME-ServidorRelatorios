using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterTotalAulasTurmaEBimestreEComponenteCurricularQueryHandler : IRequestHandler<ObterTotalAulasTurmaEBimestreEComponenteCurricularQuery, IEnumerable<TurmaComponenteQtdAulasDto>>
    {
        private readonly IRegistroFrequenciaRepository registroFrequenciaRepository;

        public ObterTotalAulasTurmaEBimestreEComponenteCurricularQueryHandler(IRegistroFrequenciaRepository registroFrequenciaRepository)
        {
            this.registroFrequenciaRepository = registroFrequenciaRepository ?? throw new ArgumentNullException(nameof(registroFrequenciaRepository));
        }

        public async Task<IEnumerable<TurmaComponenteQtdAulasDto>> Handle(ObterTotalAulasTurmaEBimestreEComponenteCurricularQuery request, CancellationToken cancellationToken)
            => await registroFrequenciaRepository.ObterTotalAulasPorDisciplinaETurmaEBimestre(request.TurmasCodigo, request.ComponentesCurricularesId, request.TipoCalendarioId, request.Bimestres);
    }
}
