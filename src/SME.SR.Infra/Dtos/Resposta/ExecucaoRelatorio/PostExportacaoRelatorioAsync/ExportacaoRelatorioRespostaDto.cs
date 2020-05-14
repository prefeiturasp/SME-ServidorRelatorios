using Newtonsoft.Json;
using SME.SR.Infra.Dtos.Resposta.ExecucaoRelatorio.GetDetalhesExecucaoRelatorio;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra.Dtos.Resposta.ExecucaoRelatorio.PostExportacaoRelatorioAsync
{
    public class ExportacaoRelatorioRespostaDto
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("outputResource")]
        public SaidaRecursoDto SaidaRecurso { get; set; }
    }
}
