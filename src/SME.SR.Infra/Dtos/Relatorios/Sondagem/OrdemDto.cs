using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra.Dtos.Relatorios.Sondagem
{
    public class OrdemDto
    {
        [JsonProperty("ordem")]
        public string Ordem { get; set; }

        [JsonProperty("ideia")]
        public string Ideia { get; set; }

        [JsonProperty("resultado")]
        public string Resultado { get; set; }
    }
}
