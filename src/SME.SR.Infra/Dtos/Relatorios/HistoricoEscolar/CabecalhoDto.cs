using Newtonsoft.Json;

namespace SME.SR.Infra.Dtos.Relatorios.HistoricoEscolar
{
    public class CabecalhoDto
    {
        [JsonProperty("nomeUe")]
        public string nomeUe { get; set; }
        [JsonProperty("endereco")]
        public string endereco { get; set; }
        [JsonProperty("atoCriacao")]
        public string atoCriacao { get; set; }
        [JsonProperty("atoAutorizacao")]
        public string atoAutorizacao { get; set; }
        [JsonProperty("lei")]
        public string lei { get; set; }
    }
}
