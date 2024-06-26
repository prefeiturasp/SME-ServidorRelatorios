﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra.Dtos.FrequenciaMensal
{
    public class ControleFrequenciaPorTipoDto
    {
        public ControleFrequenciaPorTipoDto()
        {
            FrequenciaPorAula = new List<ControleFrequenciaPorAulaDto>();
        }
        public string TipoFrequencia { get; set; }
        public int TotalDoPeriodo { get; set; }
        public List<ControleFrequenciaPorAulaDto> FrequenciaPorAula { get; set; }
    }
}
