using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra.Dtos.Requisicao.ExecucaoRelatorio.PostModificarParametrosRelatorio
{
   public class ParametroDto
    {
        [JsonProperty("name")]
        public string Nome { get; set; }

        [JsonProperty("value")]
        public string[] Valor { get; set; }
    }
}
