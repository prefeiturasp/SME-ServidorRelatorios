using Newtonsoft.Json;

namespace SME.SR.Infra.Dtos.Relatorios.HistoricoEscolar
{
    public class ParecerConclusivoDto
    {
        [JsonProperty("notaConceitoPrimeiroAno")]
        public string notaConceitoPrimeiroAno { get; set; }
        [JsonProperty("notaConceitoSegundoAno")]
        public string notaConceitoSegundoAno { get; set; }
        [JsonProperty("notaConceitoTerceiroAno")]
        public object notaConceitoTerceiroAno { get; set; }
        [JsonProperty("notaConceitoQuartoAno")]
        public string notaConceitoQuartoAno { get; set; }
        [JsonProperty("notaConceitoQuintoAno")]
        public object notaConceitoQuintoAno { get; set; }
        [JsonProperty("notaConceitoSextoAno")]
        public object notaConceitoSextoAno { get; set; }
        [JsonProperty("notaConceitoSetimoAno")]
        public string notaConceitoSetimoAno { get; set; }
        [JsonProperty("notaConceitoOitavoAno")]
        public string notaConceitoOitavoAno { get; set; }
        [JsonProperty("notaConceitoNonoAno")]
        public string notaConceitoNonoAno { get; set; }

    }
}
