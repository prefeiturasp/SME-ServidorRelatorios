using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class AulasTitularCJDataControleGradeSinteticoDto
    {
        public string Data { get; set; }
        public IEnumerable<AulasTitularCJControleGradeSinteticoDto> Divergencias { get; set; }
    }
}
