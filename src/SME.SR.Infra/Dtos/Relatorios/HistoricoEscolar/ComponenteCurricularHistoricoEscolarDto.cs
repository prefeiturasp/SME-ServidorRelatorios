
using Newtonsoft.Json;

namespace SME.SR.Infra
{
   public class ComponenteCurricularHistoricoEscolarDto
    {
        [JsonIgnore]
        public string Codigo { get; set; }

        [JsonIgnore]
        public bool Nota { get; set; }

        [JsonIgnore]
        public bool Frequencia { get; set; }

        [JsonProperty("nome")]
        public string Nome { get; set; }

        [JsonProperty("NotaConceitoPrimeiroAno")]
        public string NotaConceitoPrimeiroAno { get; set; }

        [JsonProperty("FrequenciaPrimeiroAno")]
        public string FrequenciaPrimeiroAno { get; set; }

        [JsonProperty("NotaConceitoSegundoAno")]
        public string NotaConceitoSegundoAno { get; set; }

        [JsonProperty("FrequenciaSegundoAno")]
        public string FrequenciaSegundoAno { get; set; }

        [JsonProperty("NotaConceitoTerceiroAno")]
        public string NotaConceitoTerceiroAno { get; set; }

        [JsonProperty("FrequenciaTerceiroAno")]
        public string FrequenciaTerceiroAno { get; set; }

        [JsonProperty("NotaConceitoQuartoAno")]
        public string NotaConceitoQuartoAno { get; set; }

        [JsonProperty("FrequenciaQuartoAno")]
        public string FrequenciaQuartoAno { get; set; }

        [JsonProperty("NotaConceitoQuintoAno")]
        public string NotaConceitoQuintoAno { get; set; }

        [JsonProperty("FrequenciaQuartoAno")]
        public string FrequenciaQuintoAno { get; set; }

        [JsonProperty("NotaConceitoSextoAno")]
        public string NotaConceitoSextoAno { get; set; }

        [JsonProperty("FrequenciaSextoAno")]
        public string FrequenciaSextoAno { get; set; }

        [JsonProperty("NotaConceitoSetimoAno")]
        public string NotaConceitoSetimoAno { get; set; }

        [JsonProperty("FrequenciaSetimoAno")]
        public string FrequenciaSetimoAno { get; set; }

        [JsonProperty("NotaConceitoOitavoAno")]
        public string NotaConceitoOitavoAno { get; set; }

        [JsonProperty("FrequenciaOitavoAno")]
        public string FrequenciaOitavoAno { get; set; }

        [JsonProperty("NotaConceitoNonoAno")]
        public string NotaConceitoNonoAno { get; set; }

        [JsonProperty("FrequenciaNonoAno")]
        public string FrequenciaNonoAno { get; set; }
    }
}
