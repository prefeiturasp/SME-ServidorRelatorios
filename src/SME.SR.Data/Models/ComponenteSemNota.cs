using Newtonsoft.Json;

namespace SME.SR.Data
{
    public class ComponenteSemNota
    {
        [JsonProperty("Componente")]
        public string Componente { get; set; }

        [JsonProperty("Faltas")]
        public int? Faltas { get; set; }

        [JsonProperty("Frequencia")]
        public double? Frequencia { get; set; }
    }
}
