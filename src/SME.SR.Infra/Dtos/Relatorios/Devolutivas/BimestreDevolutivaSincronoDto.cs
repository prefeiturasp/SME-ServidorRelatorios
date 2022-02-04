using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class BimestreDevolutivaSincronoDto
    {
        public string NomeBimestre { get; set; }
        public IEnumerable<DevolutivaRelatorioSincronoDto> Devolutivas { get; set; }
    }
}
