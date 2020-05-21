using Newtonsoft.Json;

namespace SME.SR.Infra.Dtos
{
    public class UnidadeOlapRecursoDto : DetalhesRecursoDto
    {
        [JsonProperty("mdxQuery")]
        public string QueryMdx { get; set; }

        [JsonProperty("olapConnection")]
        public ConexaoOlapRecursoDto ConexaoOlap { get; set; }
    }
}
