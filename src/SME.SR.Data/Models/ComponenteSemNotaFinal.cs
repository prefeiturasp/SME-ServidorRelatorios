using Newtonsoft.Json;
using SME.SR.Infra;

namespace SME.SR.Data
{
    public class ComponenteSemNotaFinal : ComponenteSemNota
    {
        [JsonProperty("Parecer")]
        public string Parecer { get; set; }
    }
}
