using Newtonsoft.Json;

namespace SME.SR.Data
{
    public class NotaBimestre
    {
        [JsonProperty("NotaConceito")]
        public string NotaConceito { get; set; }

        [JsonProperty("Bimestre")]
        public int Bimestre { get; set; }
    }
}
