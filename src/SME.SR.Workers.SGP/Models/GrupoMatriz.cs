using Newtonsoft.Json;

namespace SME.SR.Workers.SGP.Models
{
    public class GrupoMatriz
    {
        [JsonProperty("grupoMatriz")]
        public string Nome { get; set; }
    }
}
