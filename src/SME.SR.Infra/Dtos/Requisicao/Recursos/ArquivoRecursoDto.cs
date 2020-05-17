using Newtonsoft.Json;

namespace SME.SR.Infra.Dtos
{
    public class ArquivoRecursoDto : DetalhesRecursoDto
    {
        [JsonProperty("content")]
        public string Conteudo { get; set; }
    }
}
