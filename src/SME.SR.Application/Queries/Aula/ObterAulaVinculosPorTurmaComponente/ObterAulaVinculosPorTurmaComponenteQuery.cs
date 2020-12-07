using MediatR;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Application
{
    public class ObterAulaVinculosPorTurmaComponenteQuery : IRequest<IEnumerable<AulaVinculosDto>>
    {
        public ObterAulaVinculosPorTurmaComponenteQuery(long[] turmasId, string[] componenteCurricularCodigo, bool professorCJ = false)
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
