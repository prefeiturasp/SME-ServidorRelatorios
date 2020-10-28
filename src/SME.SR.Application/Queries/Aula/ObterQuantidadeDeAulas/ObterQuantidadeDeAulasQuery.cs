using MediatR;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Application
{
    public class ObterQuantidadeDeAulasQuery : IRequest<IEnumerable<AulaReduzidaDto>>
    {
        public ObterQuantidadeDeAulasQuery(long turmaId, long tipoCalendarioId, long componenteCurricularCodigo, int bimestre, bool professorCJ = false)
        {
            TurmaId = turmaId;
            TipoCalendarioId = tipoCalendarioId;
            ComponenteCurricularCodigo = componenteCurricularCodigo;
            Bimestre = bimestre;
            ProfessorCJ = professorCJ;
        }

        public long TurmaId { get; set; }
        public bool ProfessorCJ { get; set; }
        public long ComponenteCurricularCodigo { get; set; }
        public long TipoCalendarioId { get; set; }
        public int Bimestre { get; set; }
    }
}
