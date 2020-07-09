using Newtonsoft.Json;

namespace SME.SR.Infra
{
    public class EnriquecimentoCurricularDto
    {
        public string Codigo { get; set; }
        [JsonProperty("nome")]
        public string Nome { get; set; }
        [JsonIgnore]
        public bool Nota { get; set; }
        [JsonIgnore]
        public bool Frequencia { get; set; }
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
