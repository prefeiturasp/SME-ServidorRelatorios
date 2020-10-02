using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra.Dtos.Relatorios.Sondagem
{
    public class ComponenteCurricularDto
    {
        [JsonProperty("codigo")]
        public string Codigo { get; set; }

        [JsonProperty("nome")]
        public string Nome { get; set; }
    }
}
