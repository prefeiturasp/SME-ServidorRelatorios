
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;

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

        [JsonProperty("notaConceitoPrimeiroAno")]
        public string NotaConceitoPrimeiroAno { get; set; }

        [JsonProperty("frequenciaPrimeiroAno")]
        public string FrequenciaPrimeiroAno { get; set; }

        [JsonProperty("notaConceitoSegundoAno")]
        public string NotaConceitoSegundoAno { get; set; }

        [JsonProperty("frequenciaSegundoAno")]
        public string FrequenciaSegundoAno { get; set; }

        [JsonProperty("notaConceitoTerceiroAno")]
        public string NotaConceitoTerceiroAno { get; set; }

        [JsonProperty("frequenciaTerceiroAno")]
        public string FrequenciaTerceiroAno { get; set; }

        [JsonProperty("notaConceitoQuartoAno")]
        public string NotaConceitoQuartoAno { get; set; }

        [JsonProperty("frequenciaQuartoAno")]
        public string FrequenciaQuartoAno { get; set; }

        [JsonProperty("notaConceitoQuintoAno")]
        public string NotaConceitoQuintoAno { get; set; }

        [JsonProperty("frequenciaQuintoAno")]
        public string FrequenciaQuintoAno { get; set; }

        [JsonProperty("notaConceitoSextoAno")]
        public string NotaConceitoSextoAno { get; set; }

        [JsonProperty("frequenciaSextoAno")]
        public string FrequenciaSextoAno { get; set; }

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
