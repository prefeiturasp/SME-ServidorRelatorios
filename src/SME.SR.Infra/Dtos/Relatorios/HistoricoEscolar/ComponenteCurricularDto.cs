using Newtonsoft.Json;

namespace SME.SR.Infra.Dtos.Relatorios.HistoricoEscolar
{
        public class ComponenteCurricularDto
        {
            [JsonProperty("nome")]
            public string nome { get; set; }
            [JsonProperty("tipoNota")]
            public string tipoNota { get; set; }
            [JsonProperty("notaConceitoPrimeiroAno")]
            public string notaConceitoPrimeiroAno { get; set; }
            [JsonProperty("frequenciaPrimeiroAno")]
            public string frequenciaPrimeiroAno { get; set; }
            [JsonProperty("notaConceitoSegundoAno")]
            public string notaConceitoSegundoAno { get; set; }
            [JsonProperty("frequenciaSegundoAno")]
            public string frequenciaSegundoAno { get; set; }
            [JsonProperty("notaConceitoTerceiroAno")]
            public object notaConceitoTerceiroAno { get; set; }
            [JsonProperty("frequenciaTerceiroAno")]
            public object frequenciaTerceiroAno { get; set; }
            [JsonProperty("notaConceitoQuartoAno")]
            public string notaConceitoQuartoAno { get; set; }
            [JsonProperty("frequenciaQuartoAno")]
            public string frequenciaQuartoAno { get; set; }
            [JsonProperty("notaConceitoQuintoAno")]
            public object notaConceitoQuintoAno { get; set; }
            [JsonProperty("frequenciaQuintoAno")]
            public object frequenciaQuintoAno { get; set; }
            [JsonProperty("notaConceitoSextoAno")]
            public object notaConceitoSextoAno { get; set; }
            [JsonProperty("frequenciaSextoAno")]
            public object frequenciaSextoAno { get; set; }
            [JsonProperty("notaConceitoSetimoAno")]
            public string notaConceitoSetimoAno { get; set; }
            [JsonProperty("frequenciaSetimoAno")]
            public string frequenciaSetimoAno { get; set; }
            [JsonProperty("notaConceitoOitavoAno")]
            public string notaConceitoOitavoAno { get; set; }
            [JsonProperty("frequenciaOitavoAno")]
            public string frequenciaOitavoAno { get; set; }
            [JsonProperty("notaConceitoNonoAno")]
            public string notaConceitoNonoAno { get; set; }
            [JsonProperty("frequenciaNonoAno")]
            public string frequenciaNonoAno { get; set; }

        }
}
