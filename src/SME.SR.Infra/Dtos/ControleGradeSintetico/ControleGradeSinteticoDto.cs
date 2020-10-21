using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class ControleGradeSinteticoDto
    {
        public ControleGradeSinteticoDto()
        {
            Filtro = new FiltroGradeSintetico();
            Turmas = new List<TurmaControleGradeSinteticoDto>();
        }

        public FiltroGradeSintetico Filtro { get; set; }
       
        public List<TurmaControleGradeSinteticoDto> Turmas { get; set; }
    }
}
