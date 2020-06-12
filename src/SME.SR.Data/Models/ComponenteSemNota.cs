using Newtonsoft.Json;

namespace SME.SR.Data
{
    public class ComponenteSemNota
    {
        [JsonProperty("componente")]
        public string Componente { get; set; }

        [JsonProperty("faltas")]
        public int? Faltas { get; set; }

        [JsonProperty("frequencia")]
        public double? Frequencia { get; set; }
    }
}
