using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra.Dtos.Resposta.ControlesEntrada
{
    public class TipoDadosDto
    {
        [JsonProperty("type")]
        public string Tipo { get; set; }

        [JsonProperty("maxValue")]
        public string ValorMaximo { get; set; }

        [JsonProperty("strictMax")]
        public bool MaximoEstrito { get; set; }

        [JsonProperty("minValue")]
        public string ValorMinimo { get; set; }

        [JsonProperty("strictMin")]
        public bool MinimoEstrito { get; set; }
    }
}
