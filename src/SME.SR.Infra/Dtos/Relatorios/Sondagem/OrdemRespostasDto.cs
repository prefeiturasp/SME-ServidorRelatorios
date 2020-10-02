using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra.Dtos.Relatorios.Sondagem
{
    public class OrdemRespostasDto
    {
        [JsonProperty("ordemId")]
        public string OrdemId { get; set; }

        [JsonProperty("ideia")]
        public string Ideia { get; set; }

        [JsonProperty("resultado")]
        public string Resultado { get; set; }
    }
}
