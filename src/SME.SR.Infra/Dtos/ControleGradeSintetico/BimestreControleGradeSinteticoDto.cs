using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class BimestreControleGradeSinteticoDto
    {
        public BimestreControleGradeSinteticoDto()
        {
            ComponentesCurriculares = new List<ComponenteCurricularControleGradeSinteticoDto>();
        }

        public string Descricao { get; set; }
        public List<ComponenteCurricularControleGradeSinteticoDto> ComponentesCurriculares { get; set; }
    }
}
