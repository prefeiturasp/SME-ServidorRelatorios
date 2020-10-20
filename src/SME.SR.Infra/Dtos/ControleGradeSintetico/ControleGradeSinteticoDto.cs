using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class ControleGradeSinteticoDto
    {

        public FiltroGradeSintetico Filtro { get; set; }
       
        public IEnumerable<TurmaControleGradeSinteticoDto> Turmas { get; set; }
    }
}
