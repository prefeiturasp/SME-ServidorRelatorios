using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Application
{
    public class VerificaExisteMaisAulaCadastradaNoDiaQuery : IRequest<bool>
    {
        public VerificaExisteMaisAulaCadastradaNoDiaQuery(long turmaId, string componenteCurricularId, long tipoCalendarioId, int bimestre)
        {
            TurmaId = turmaId;
            ComponenteCurricularId = componenteCurricularId;
            Bimestre = bimestre;
            TipoCalendarioId = tipoCalendarioId;
        }

        public long TurmaId { get; set; }
        public string ComponenteCurricularId { get; set; }
        public int Bimestre { get; set; }
        public long TipoCalendarioId { get; set; }
    }
}
