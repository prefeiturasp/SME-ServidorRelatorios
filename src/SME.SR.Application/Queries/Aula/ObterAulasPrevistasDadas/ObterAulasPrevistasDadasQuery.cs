using MediatR;
using SME.SR.Data;

namespace SME.SR.Application
{
    public class ObterAulasPrevistasDadasQuery : IRequest<AulaPrevista>
    {
        public ObterAulasPrevistasDadasQuery(long tipoCalendarioId, string turmaId, string disciplinaId)
        {
            TipoCalendarioId = tipoCalendarioId;
            TurmaId = turmaId;
            DisciplinaId = disciplinaId;
            DisciplinaId = disciplinaId;
        }
        public long TipoCalendarioId { get; set; }
        public string TurmaId { get; set; }
        public string DisciplinaId { get; set; }
    }
}
