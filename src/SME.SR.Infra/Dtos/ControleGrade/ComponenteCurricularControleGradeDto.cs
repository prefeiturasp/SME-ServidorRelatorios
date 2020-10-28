using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class ComponenteCurricularControleGradeDto
    {
        public ComponenteCurricularControleGradeDto()
        {
            VisaoSemanal = new List<VisaoSemanalControleGradeSinteticoDto>();
        }
        public string Nome { get; set; }
        public long AulasPrevistas { get; set; }
        public long AulasCriadasProfessorTitular { get; set; }
        public long AulasCriadasProfessorSubstituto { get; set; }
        public long AulasDadasProfessorTitular { get; set; }
        public long AulasDadasProfessorSubstituto { get; set; }
        public long Repostas { get; set; }
        public string Divergencias { get; set; }
        public DetalhamentoDivergenciasControleGradeSinteticoDto DetalhamentoDivergencias {get;set;}
        public IEnumerable<VisaoSemanalControleGradeSinteticoDto> VisaoSemanal { get; set; }
    }
}
