using Newtonsoft.Json;

namespace SME.SR.Workers.SGP.Models
{
    public class NotaBimestre
    {
        [JsonProperty("notaConceito")]
        public string NotaConceito { get; set; }

        [JsonProperty("bimestre")]
        public int Bimestre { get; set; }
    }
}
