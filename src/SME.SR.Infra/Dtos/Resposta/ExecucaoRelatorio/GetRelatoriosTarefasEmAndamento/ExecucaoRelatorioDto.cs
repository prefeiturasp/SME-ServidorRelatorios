using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra.Dtos.Resposta
{
    public class ExecucaoRelatorioDto
    {
        [JsonProperty("reportURI")]
        public string RelatorioUri { get; set; }

        [JsonProperty("requestId")]
        public string RequisicaoId { get; set; }
    }
}
