using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class TurmasDevolutivasDto
    {
        public string NomeTurma { get; set; }
        public IEnumerable<BimestresDevolutivasDto> Bimestres { get; set; }
    }
}
