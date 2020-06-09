using Newtonsoft.Json;

namespace SME.SR.Workers.SGP.Models
{
    public class ComponenteSemNotaFinal : ComponenteSemNota
    {
        [JsonProperty("Parecer")]
        public string Parecer { get; set; }
    }
}
