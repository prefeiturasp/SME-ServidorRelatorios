using Newtonsoft.Json;
using System;

namespace SME.SR.Infra.Dtos.Resposta
{
    public class DetalhesExecucaoRelatorioRespostaDto
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("totalPages")]
        public long TotalPaginas { get; set; }

        [JsonProperty("requestId")]
        public Guid RequisicaoId { get; set; }

        [JsonProperty("reportURI")]
        public string RelatorioUri { get; set; }

        [JsonProperty("exports")]
        public ExportacaoDetalhesDto[] Exportacoes { get; set; }
        public bool Pronto => !string.IsNullOrWhiteSpace(Status) && Status.Equals("ready");
        public string JSessionId { get; set; }
        public void AdicionarJSessionId(string jSessionId)
        {
            JSessionId = jSessionId;
        }
    }
}
