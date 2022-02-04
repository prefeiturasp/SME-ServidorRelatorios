using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class TurmaDevolutivaSincronoDto
    {
        public string NomeTurma { get; set; }
        public IEnumerable<BimestreDevolutivaSincronoDto> Bimestres { get; set; }
    }
}
