using Newtonsoft.Json;

namespace SME.SR.Data
{
    public class NotaBimestre
    {
        [JsonProperty("notaConceito")]
        public string NotaConceito { get; set; }

        [JsonProperty("bimestre")]
        public int Bimestre { get; set; }
    }
}
