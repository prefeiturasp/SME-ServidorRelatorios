using Newtonsoft.Json;

namespace SME.SR.Infra
{
    public class DadosDataDto
    {
        [JsonProperty("municipio")]
        public string Municipio { get; set; }

        [JsonProperty("dia")]
        public string Dia { get; set; }

        [JsonProperty("mes")]
        public string Mes { get; set; }

        [JsonProperty("ano")]
        public string Ano { get; set; }

    }
}
