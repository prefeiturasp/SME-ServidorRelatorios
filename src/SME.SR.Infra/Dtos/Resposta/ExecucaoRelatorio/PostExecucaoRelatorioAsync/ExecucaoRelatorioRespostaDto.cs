using Newtonsoft.Json;
using System;

namespace SME.SR.Infra.Dtos.Resposta
{
    public class ExecucaoRelatorioRespostaDto
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("requestId")]
        public Guid RequestId { get; set; }

        [JsonProperty("reportURI")]
        public string ReportUri { get; set; }

        [JsonProperty("exports")]
        public ExportacaoDto[] Exports { get; set; }
        public string JSessionId { get; set; }
        public void AdicionarJSessionId(string jSessionId)
        {
            if (!string.IsNullOrWhiteSpace(jSessionId))
                JSessionId = jSessionId.Substring(11, 32);
        }
    }
}
