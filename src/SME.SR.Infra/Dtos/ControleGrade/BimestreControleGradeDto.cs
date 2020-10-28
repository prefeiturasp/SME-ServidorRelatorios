using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class BimestreControleGradeDto
    {
        public BimestreControleGradeDto()
        {
            ComponentesCurriculares = new List<ComponenteCurricularControleGradeDto>();
        }

        public string Descricao { get; set; }
        public List<ComponenteCurricularControleGradeDto> ComponentesCurriculares { get; set; }
    }
}
