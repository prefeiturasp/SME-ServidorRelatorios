using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class BimestreControleGradeSinteticoDto
    {
        public string Descricao { get; set; }
        public IEnumerable<ComponenteCurricularControleGradeSinteticoDto> ComponentesCurriculares { get; set; }
    }
}
