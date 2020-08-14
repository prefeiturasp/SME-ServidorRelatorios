using Newtonsoft.Json;

namespace SME.SR.Infra
{
    public class TiposNotaEJADto
    {
        [JsonProperty("primeiraEtapaCiclo1")]
        public string PrimeiraEtapaCiclo1 { get; set; }

        [JsonProperty("segundaEtapaCiclo1")]
        public string SegundaEtapaCiclo1 { get; set; }

        [JsonProperty("primeiraEtapaCiclo2")]
        public string PrimeiraEtapaCiclo2 { get; set; }

        [JsonProperty("segundaEtapaCiclo2")]
        public string SegundaEtapaCiclo2 { get; set; }

        [JsonProperty("primeiraEtapaCiclo3")]
        public string PrimeiraEtapaCiclo3 { get; set; }

        [JsonProperty("segundaEtapaCiclo3")]
        public string SegundaEtapaCiclo3 { get; set; }

        [JsonProperty("primeiraEtapaCiclo4")]
        public string PrimeiraEtapaCiclo4 { get; set; }

        [JsonProperty("segundaEtapaCiclo4")]
        public string SegundaEtapaCiclo4 { get; set; }


    }
}
