using Newtonsoft.Json;

namespace SME.SR.Infra
{
    public class ComponenteCurricularHistoricoEscolarEJADto
    {
        [JsonIgnore]
        public string Codigo { get; set; }

        [JsonIgnore]
        public bool Nota { get; set; }

        [JsonIgnore]
        public bool Frequencia { get; set; }

        [JsonProperty("nome")]
        public string Nome { get; set; }

        [JsonProperty("notaConceitoPrimeiraEtapaCiclo1")]
        public string NotaConceitoPrimeiraEtapaCiclo1 { get; set; }

        [JsonProperty("frequenciaPrimeiraEtapaCiclo1")]
        public string FrequenciaPrimeiraEtapaCiclo1 { get; set; }

        [JsonProperty("notaConceitoSegundaEtapaCiclo1")]
        public string NotaConceitoSegundaEtapaCiclo1 { get; set; }

        [JsonProperty("frequenciaSegundaEtapaCiclo1")]
        public string FrequenciaSegundaEtapaCiclo1 { get; set; }


        [JsonProperty("notaConceitoPrimeiraEtapaCiclo2")]
        public string NotaConceitoPrimeiraEtapaCiclo2 { get; set; }

        [JsonProperty("frequenciaPrimeiraEtapaCiclo2")]
        public string FrequenciaPrimeiraEtapaCiclo2 { get; set; }

        [JsonProperty("notaConceitoSegundaEtapaCiclo2")]
        public string NotaConceitoSegundaEtapaCiclo2 { get; set; }

        [JsonProperty("frequenciaSegundaEtapaCiclo2")]
        public string FrequenciaSegundaEtapaCiclo2 { get; set; }


        [JsonProperty("notaConceitoPrimeiraEtapaCiclo3")]
        public string NotaConceitoPrimeiraEtapaCiclo3 { get; set; }

        [JsonProperty("frequenciaPrimeiraEtapaCiclo3")]
        public string FrequenciaPrimeiraEtapaCiclo3 { get; set; }

        [JsonProperty("notaConceitoSegundaEtapaCiclo3")]
        public string NotaConceitoSegundaEtapaCiclo3 { get; set; }

        [JsonProperty("frequenciaSegundaEtapaCiclo3")]
        public string FrequenciaSegundaEtapaCiclo3 { get; set; }


        [JsonProperty("notaConceitoPrimeiraEtapaCiclo4")]
        public string NotaConceitoPrimeiraEtapaCiclo4 { get; set; }

        [JsonProperty("frequenciaPrimeiraEtapaCiclo4")]
        public string FrequenciaPrimeiraEtapaCiclo4 { get; set; }

        [JsonProperty("notaConceitoSegundaEtapaCiclo4")]
        public string NotaConceitoSegundaEtapaCiclo4 { get; set; }

        [JsonProperty("frequenciaSegundaEtapaCiclo4")]
        public string FrequenciaSegundaEtapaCiclo4 { get; set; }


        [JsonIgnore]
        public bool PossuiNotaValida
        {
            get
            {
                if (Nota && (!string.IsNullOrEmpty(NotaConceitoPrimeiraEtapaCiclo1) ||
                    !string.IsNullOrEmpty(NotaConceitoPrimeiraEtapaCiclo2) ||
                    !string.IsNullOrEmpty(NotaConceitoPrimeiraEtapaCiclo3) ||
                    !string.IsNullOrEmpty(NotaConceitoPrimeiraEtapaCiclo4) ||
                    !string.IsNullOrEmpty(NotaConceitoSegundaEtapaCiclo1) ||
                    !string.IsNullOrEmpty(NotaConceitoSegundaEtapaCiclo2) ||
                    !string.IsNullOrEmpty(NotaConceitoSegundaEtapaCiclo3) ||
                    !string.IsNullOrEmpty(NotaConceitoSegundaEtapaCiclo4)))
                    return true;

                return false;
            }
        }
    }
}
