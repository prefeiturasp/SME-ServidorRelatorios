using Newtonsoft.Json;

namespace SME.SR.Data
{
    public class ComponenteSemNotaFinal : ComponenteSemNota
    {
        [JsonProperty("parecer")]
        public string Parecer { get; set; }
    }
}
