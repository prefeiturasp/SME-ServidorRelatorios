using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra.Dtos.Resposta.ExecucaoRelatorio.GetDetalhesExecucaoRelatorio
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
        public ExportacaoDto[] Exportacoes { get; set; }
    }
}
