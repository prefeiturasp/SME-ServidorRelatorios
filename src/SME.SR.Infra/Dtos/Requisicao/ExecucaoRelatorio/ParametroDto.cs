using Newtonsoft.Json;

namespace SME.SR.Infra.Dtos.Requisicao
{
    public class ParametroDto
    {
        [JsonProperty("name")]
        public string Nome { get; set; }

        [JsonProperty("value")]
        public string[] Valor { get; set; }
    }
}
