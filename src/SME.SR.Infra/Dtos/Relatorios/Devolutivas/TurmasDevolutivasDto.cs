using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class TurmasDevolutivasDto
    {
        public string NomeTurma { get; set; }
        public List<BimestresDevolutivasDto> Bimestres { get; set; }
    }
}
