using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra.Dtos.Requisicao.ExecucaoRelatorio.PostExportacaoRelatorioAsync
{
    public class ExportacaoRelatorioRequisicaoDto
    {
        [JsonProperty("outputFormat")]
        public string FormatoSaida { get; set; }

        [JsonProperty("pages")]
        public string Paginas { get; set; }

        [JsonProperty("attachmentsPrefix")]
        public string AnexosPrefixo { get; set; }
    }
}
