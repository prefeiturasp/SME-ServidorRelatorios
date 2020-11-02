using MediatR;
using SME.SR.Data;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterAulasPrevistasDadasQuery : IRequest<IEnumerable<AulaPrevistaBimestreQuantidade>>
    {
        public ObterAulasPrevistasDadasQuery(long turmaId, long componenteCurricularId, long tipoCalendarioId)
        {
            TurmaId = turmaId;
            ComponenteCurricularId = componenteCurricularId;
            TipoCalendarioId = tipoCalendarioId;
        }

        public long TurmaId { get; set; }
        public long ComponenteCurricularId { get; set; }
        public long TipoCalendarioId { get; set; }
    }
}
