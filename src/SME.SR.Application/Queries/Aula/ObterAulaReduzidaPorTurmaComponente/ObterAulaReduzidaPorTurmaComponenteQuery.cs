using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterAulaReduzidaPorTurmaComponenteQuery : IRequest<IEnumerable<AulaReduzidaDto>>
    {
        public ObterAulaReduzidaPorTurmaComponenteQuery(long[] turmasId, string[] componenteCurricularCodigo, bool professorCJ = false)
        {
            TurmasId = turmasId;
            ComponenteCurricularesCodigo = componenteCurricularCodigo;
            ProfessorCJ = professorCJ;
        }

        public long[] TurmasId { get; set; }
        public bool ProfessorCJ { get; set; }
        public string[] ComponenteCurricularesCodigo { get; set; }
    }
}
