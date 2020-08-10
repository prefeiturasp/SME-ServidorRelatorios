using Newtonsoft.Json;

namespace SME.SR.Infra
{
    public class EnsinoReligiosoEJADto
    {
        [JsonProperty("notaConceitoPrimeiraEtapaCiclo1")]
        public string NotaConceitoPrimeiraEtapaCiclo1 { get; set; }

        [JsonProperty("notaConceitoSegundaEtapaCiclo1")]
        public string NotaConceitoSegundaEtapaCiclo1 { get; set; }


        [JsonProperty("notaConceitoPrimeiraEtapaCiclo2")]
        public string NotaConceitoPrimeiraEtapaCiclo2 { get; set; }

        [JsonProperty("notaConceitoSegundaEtapaCiclo2")]
        public string NotaConceitoSegundaEtapaCiclo2 { get; set; }


        [JsonProperty("notaConceitoPrimeiraEtapaCiclo3")]
        public string NotaConceitoPrimeiraEtapaCiclo3 { get; set; }

        [JsonProperty("notaConceitoSegundaEtapaCiclo3")]
        public string NotaConceitoSegundaEtapaCiclo3 { get; set; }


        [JsonProperty("notaConceitoPrimeiraEtapaCiclo4")]
        public string NotaConceitoPrimeiraEtapaCiclo4 { get; set; }

        [JsonProperty("notaConceitoSegundaEtapaCiclo4")]
        public string NotaConceitoSegundaEtapaCiclo4 { get; set; }
    }
}
