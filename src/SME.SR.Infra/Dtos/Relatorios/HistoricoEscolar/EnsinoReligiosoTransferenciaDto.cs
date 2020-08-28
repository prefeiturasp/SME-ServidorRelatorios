using Newtonsoft.Json;

namespace SME.SR.Infra
{
    public class EnsinoReligiosoTransferenciaDto
    {
        [JsonProperty("notaConceitoPrimeiroBimestre")]
        public string NotaConceitoPrimeiroBimestre { get; set; }

        [JsonProperty("notaConceitoSegundoBimestre")]
        public string NotaConceitoSegundoBimestre { get; set; }

        [JsonProperty("notaConceitoTerceiroBimestre")]
        public string NotaConceitoTerceiroBimestre { get; set; }

        [JsonProperty("notaConceitoQuartoBimestre")]
        public string NotaConceitoQuartoBimestre { get; set; }
    }
}
