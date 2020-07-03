using Newtonsoft.Json;

namespace SME.SR.Infra
{
    public class CabecalhoDto
    {
        [JsonProperty("nomeUe")]
        public string NomeUe { get; set; }
        [JsonProperty("endereco")]
        public string Endereco { get; set; }
        [JsonProperty("toCriacao")]
        public string AtoCriacao { get; set; }
        [JsonProperty("atoAutorizacao")]
        public string AtoAutorizacao { get; set; }
        [JsonProperty("lei")]
        public string Lei { get; set; }
    }
}
