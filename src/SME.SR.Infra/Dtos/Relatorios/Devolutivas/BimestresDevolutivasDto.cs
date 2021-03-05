using System;
using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class BimestresDevolutivasDto
    {
        public string NomeBimestre { get; set; }
        public List<DevolutivasDto> Devolutivas { get; set; }

    }
}
