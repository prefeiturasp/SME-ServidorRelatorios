using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra.Dtos
{
    public class TrabalhoEstadoDto
    {
        [JsonProperty("previousFireTime")]
        public DateTime FireTimeAnterior { get; set; }

        [JsonProperty("nextFireTime")]
        public DateTime FireTimeProximo { get; set; }

        [JsonProperty("value")]
        public string Valor { get; set; }
    }
}
