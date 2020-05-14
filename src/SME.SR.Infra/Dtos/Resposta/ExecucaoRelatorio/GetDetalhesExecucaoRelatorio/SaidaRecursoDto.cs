using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra.Dtos.Resposta.ExecucaoRelatorio.GetDetalhesExecucaoRelatorio
{
    public class SaidaRecursoDto
    {
        [JsonProperty("contentType")]
        public string TipoConteudo { get; set; }
    }
}
