﻿using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioFechamentoPendenciasTurmaDto
    {
        public RelatorioFechamentoPendenciasTurmaDto()
        {
            Bimestres = new List<RelatorioFechamentoPendenciasBimestreDto>();
        }
        public string Nome { get; set; }
        public List<RelatorioFechamentoPendenciasBimestreDto> Bimestres { get; set; }

    }
}
