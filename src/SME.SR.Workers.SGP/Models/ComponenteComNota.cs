using Newtonsoft.Json;

namespace SME.SR.Workers.SGP.Models
{
    public abstract class ComponenteComNota
    {
        [JsonProperty("componente")]
        public string Componente { get; set; }

        [JsonProperty("faltas")]
        public int? Faltas { get; set; }

        [JsonProperty("ausenciasCompensadas")]
        public int? AusenciasCompensadas { get; set; }

        [JsonProperty("frequencia")]
        public double? Frequencia { get; set; }
    }
}
