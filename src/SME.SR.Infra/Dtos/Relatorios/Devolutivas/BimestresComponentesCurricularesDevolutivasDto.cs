using System;
using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class BimestresComponentesCurricularesDevolutivasDto
    {
        public string NomeBimestreComponenteCurricular { get; set; }
        public IEnumerable<DevolutivaRelatorioDto> Devolutivas { get; set; }

    }
}
