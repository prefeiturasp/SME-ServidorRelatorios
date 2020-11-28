using Newtonsoft.Json;

namespace SME.SR.Infra
{
    public  class UeConclusaoDto
    {
        [JsonProperty("ano")]
        public string Ano { get; set; }
        [JsonProperty("ueNome")]
        public string UeNome { get; set; }
        [JsonProperty("ueMunicipio")]
        public string UeMunicipio { get; set; }
        [JsonProperty("ueUf")]
        public string UeUf { get; set; }
    }
}
