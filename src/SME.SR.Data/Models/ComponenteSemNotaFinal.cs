using Newtonsoft.Json;

namespace SME.SR.Data
{
    public class ComponenteSemNotaFinal : ComponenteSemNota
    {
        [JsonProperty("Parecer")]
        public string Parecer { get; set; }
    }
}
