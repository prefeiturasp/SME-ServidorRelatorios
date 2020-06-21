using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Data
{
    public abstract class ComponenteRegenciaComNota
    {
        [JsonProperty("Faltas")]
        public int? Faltas { get; set; }

        [JsonProperty("AusenciasCompensadas")]
        public int? AusenciasCompensadas { get; set; }

        [JsonProperty("Frequencia")]
        public double? Frequencia { get; set; }

        [JsonProperty("TipoNota")]
        public string TipoNota { get; set; }
    }
}
