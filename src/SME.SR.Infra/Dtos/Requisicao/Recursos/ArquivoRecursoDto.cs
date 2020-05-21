using Newtonsoft.Json;

namespace SME.SR.Infra.Dtos
{
    public class ArquivoRecursoDto : DetalhesRecursoDto
    {
        [JsonProperty("type")]
        public string Tipo { get; set; }

        [JsonProperty("content")]
        public string Conteudo { get; set; }
    }
}
