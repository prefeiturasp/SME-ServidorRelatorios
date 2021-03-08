using System;
using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class BimestresDevolutivasDto
    {
        public string NomeBimestre { get; set; }
        public List<DevolutivaRelatorioDto> Devolutivas { get; set; }

    }
}
