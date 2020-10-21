using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class TurmaControleGradeSinteticoDto
    {
        public TurmaControleGradeSinteticoDto()
        {
            Bimestres = new List<BimestreControleGradeSinteticoDto>();
        }

        public long Id { get; set; }
        public string Nome { get; set; }
        public List<BimestreControleGradeSinteticoDto> Bimestres { get; set; }
    }
}
