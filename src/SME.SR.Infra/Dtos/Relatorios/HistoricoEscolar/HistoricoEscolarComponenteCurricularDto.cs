using Newtonsoft.Json;

namespace SME.SR.Infra
{
    public class HistoricoEscolarComponenteCurricularDto
    {
        [JsonProperty("nome")]
        public string Nome { get; set; }

        [JsonProperty("notaConceitoPrimeiroAno")]
        public string NotaConceitoPrimeiroAno { get; set; }
        [JsonProperty("frequenciaPrimeiroAno")]
        public string FrequenciaPrimeiroAno { get; set; }
        [JsonProperty("notaConceitoSegundoAno")]
        public string NotaConceitoSegundoAno { get; set; }
        [JsonProperty("frequenciaSegundoAno")]
        public string FrequenciaSegundoAno { get; set; }
        [JsonProperty("notaConceitoTerceiroAno")]
        public object NotaConceitoTerceiroAno { get; set; }
        [JsonProperty("frequenciaTerceiroAno")]
        public object FrequenciaTerceiroAno { get; set; }
        [JsonProperty("notaConceitoQuartoAno")]
        public string NotaConceitoQuartoAno { get; set; }
        [JsonProperty("frequenciaQuartoAno")]
        public string FrequenciaQuartoAno { get; set; }
        [JsonProperty("notaConceitoQuintoAno")]
        public object NotaConceitoQuintoAno { get; set; }
        [JsonProperty("frequenciaQuintoAno")]
        public object FrequenciaQuintoAno { get; set; }
        [JsonProperty("notaConceitoSextoAno")]
        public object NotaConceitoSextoAno { get; set; }
        [JsonProperty("frequenciaSextoAno")]
        public object FrequenciaSextoAno { get; set; }
        [JsonProperty("notaConceitoSetimoAno")]
        public string NotaConceitoSetimoAno { get; set; }
        [JsonProperty("frequenciaSetimoAno")]
        public string FrequenciaSetimoAno { get; set; }
        [JsonProperty("notaConceitoOitavoAno")]
        public string NotaConceitoOitavoAno { get; set; }
        [JsonProperty("frequenciaOitavoAno")]
        public string FrequenciaOitavoAno { get; set; }
        [JsonProperty("notaConceitoNonoAno")]
        public string NotaConceitoNonoAno { get; set; }
        [JsonProperty("frequenciaNonoAno")]
        public string FrequenciaNonoAno { get; set; }
    }
}
