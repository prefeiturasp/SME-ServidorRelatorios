using Newtonsoft.Json;

namespace SME.SR.Infra
{
    public class ParecerConclusivoDto
    {
        [JsonProperty("notaConceitoPrimeiroAno")]
        public string NotaConceitoPrimeiroAno { get; set; }
        [JsonProperty("notaConceitoSegundoAno")]
        public string NotaConceitoSegundoAno { get; set; }
        [JsonProperty("notaConceitoTerceiroAno")]
        public object NotaConceitoTerceiroAno { get; set; }
        [JsonProperty("notaConceitoQuartoAno")]
        public string NotaConceitoQuartoAno { get; set; }
        [JsonProperty("notaConceitoQuintoAno")]
        public object NotaConceitoQuintoAno { get; set; }
        [JsonProperty("notaConceitoSextoAno")]
        public object NotaConceitoSextoAno { get; set; }
        [JsonProperty("notaConceitoSetimoAno")]
        public string NotaConceitoSetimoAno { get; set; }
        [JsonProperty("notaConceitoOitavoAno")]
        public string NotaConceitoOitavoAno { get; set; }
        [JsonProperty("notaConceitoNonoAno")]
        public string NotaConceitoNonoAno { get; set; }

    }
}
