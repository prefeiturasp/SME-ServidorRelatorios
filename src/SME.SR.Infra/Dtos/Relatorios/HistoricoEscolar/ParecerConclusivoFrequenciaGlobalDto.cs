using Newtonsoft.Json;

namespace SME.SR.Infra
{
    public class ParecerConclusivoFrequenciaGlobalDto  
    {
        [JsonProperty("primeiroAno")]
        public string PrimeiroAno { get; set; }

        [JsonProperty("segundoAno")]
        public string SegundoAno { get; set; }

        [JsonProperty("terceiroAno")]
        public string TerceiroAno { get; set; }

        [JsonProperty("quartoAno")]
        public string QuartoAno { get; set; }

        [JsonProperty("quintoAno")]
        public string QuintoAno { get; set; }

        [JsonProperty("sextoAno")]
        public string SextoAno { get; set; }

        [JsonProperty("setimoAno")]
        public string SetimoAno { get; set; }

        [JsonProperty("oitavoAno")]
        public string OitavoAno { get; set; }

        [JsonProperty("nonoAno")]
        public string NonoAno { get; set; }

    }
}
