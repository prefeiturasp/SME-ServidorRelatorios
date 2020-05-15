using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra.Dtos.Resposta
{
   public class RelatoriosTarefasEmAndamentoRespostaDto
    {
        [JsonProperty("reportExecution")]
        public ExecucaoRelatorioDto[] ExecucoesRelatorio { get; set; }
    }
}
