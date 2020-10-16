using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class TurmaControleGradeSinteticoDto
    {
        public long Id { get; set; }
        public string Nome { get; set; }
        public IEnumerable<BimestreControleGradeSinteticoDto> Bimestres { get; set; }
    }
}
