using Newtonsoft.Json;
using System;

namespace SME.SR.Infra.Dtos.Resposta.ExecucaoRelatorio.PostExecucaoRelatorioAsync
{
    public class ExecucaoRelatorioRespostaDto
    {
        [JsonProperty("currentPage")]
        public long PaginaAtual { get; set; }

        [JsonProperty("reportURI")]
        public string URIRelatorio { get; set; }

        [JsonProperty("requestId")]
        public Guid RequisicaoId { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("exports")]
        public ExportacoesDto Exportacoes { get; set; }
    }
}
