using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra.Dtos.Resposta.ControlesEntrada
{
    public class OpcaoDto
    {
        [JsonProperty("selected")]
        public bool Selecionado { get; set; }

        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("value")]
        public string Valor { get; set; }
    }
}
