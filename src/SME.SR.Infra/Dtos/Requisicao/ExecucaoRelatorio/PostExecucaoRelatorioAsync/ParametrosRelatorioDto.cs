﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra.Dtos.Requisicao
{
    public  class ParametrosRelatorioDto
    {
        [JsonProperty("reportParameter")]
        public ParametroDto[] ParametrosRelatorio { get; set; }
    }
}
