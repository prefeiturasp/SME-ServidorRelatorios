using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra.Dtos.Resposta.ExecucaoRelatorio.GetDetalhesExecucaoRelatorio
{
    public class OpcaoDto
    {
        [JsonProperty("outputFormat")]
        public string FormatoSaida { get; set; }

        [JsonProperty("attachmentsPrefix")]
        public string AnexosPrefixo { get; set; }

        [JsonProperty("allowInlineScripts")]
        public bool PermiteScripts { get; set; }

        [JsonProperty("baseUrl", NullValueHandling = NullValueHandling.Ignore)]
        public Uri BaseUrl { get; set; }
    }
}
