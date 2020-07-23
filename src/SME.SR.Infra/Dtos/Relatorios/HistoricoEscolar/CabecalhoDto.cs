using Newtonsoft.Json;

namespace SME.SR.Infra
{
    public class CabecalhoDto
    {
        [JsonProperty("nomeUe")]
        public string NomeUe { get; set; }
        [JsonProperty("endereco")]
        public string Endereco { get; set; }
        [JsonProperty("atoCriacao")]
        public string AtoCriacao { get; set; }
        [JsonProperty("atoAutorizacao")]
        public string AtoAutorizacao { get; set; }
        //TODO: Será tratado em próximas sprint para domínio de leis e modalidades
        [JsonProperty("leiFundamental")]
        public string LeiFundamental { get { return "Ensino Fundamental – Lei Federal nº 9.394/96 - Lei Federal nº 11.274/06"; }  }

        [JsonProperty("leiMedio")]
        public string LeiMedio { get { return "Ensino Médio – Lei Nº 9.394/1996 e 13.415/2017"; } }
    }
}
