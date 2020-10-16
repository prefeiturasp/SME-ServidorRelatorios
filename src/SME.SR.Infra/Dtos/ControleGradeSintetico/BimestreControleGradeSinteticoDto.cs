using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class BimestreControleGradeSinteticoDto
    {
        public long Id { get; set; }
        public string Nome { get; set; }
        public string Vigencia { get; set; }
        public IEnumerable<ComponenteCurricularControleGradeSinteticoDto> ComponentesCurriculares { get; set; }
    }
}
