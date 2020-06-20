﻿using Newtonsoft.Json;

namespace SME.SR.Data
{
    public abstract class ComponenteComNota
    {
        [JsonProperty("Componente")]
        public string Componente { get; set; }

        [JsonProperty("Faltas")]
        public int? Faltas { get; set; }

        [JsonProperty("AusenciasCompensadas")]
        public int? AusenciasCompensadas { get; set; }

        [JsonProperty("Frequencia")]
        public double? Frequencia { get; set; }

        [JsonProperty("EhEja")]
        public bool EhEja { get; set; }

        [JsonProperty("TipoNota")]
        public string TipoNota { get; set; }
    }
}
