using Newtonsoft.Json;

namespace SME.SR.Infra
{
    public class EnderecoEAtosDaUeDto 
    {
        [JsonProperty("nomeUe")]
        public string NomeUe { get; set; }
        [JsonProperty("endereco")]
        public string Endereco { get; set; }
        [JsonProperty("ato")]
        public string Atos { get; set; }
        [JsonProperty("tipoOcorrencia")]
        public string TipoOcorrencia { get; set; }
        
    }
}
