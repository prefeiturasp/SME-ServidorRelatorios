using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra.Dtos.Requisicao
{
    public class RelatoriosTarefasEmAndamentoRequisicaoDto
    {
        [JsonProperty("reportURI")]
        public string RelatorioUri { get; set; }

        [JsonProperty("jobID")]
        public string TarefaId { get; set; }

        [JsonProperty("jobLabel")]
        public string TarefaLabel { get; set; }

        [JsonProperty("userName")]
        public string UsuarioNome { get; set; }

        [JsonProperty("fireTimeFrom")]
        public DateTime DataInicio { get; set; }

        [JsonProperty("fireTimeTo")]
        public DateTime DataFim { get; set; }
    }
}
