using Newtonsoft.Json;

namespace SME.SR.Infra.Dtos
{
    public class QueryRecursoDto : DetalhesRecursoDto
    {
        [JsonProperty("value")]
        public string Valor { get; set; }

        [JsonProperty("language")]
        public string Idioma { get; set; }

        [JsonProperty("dataSource")]
        public FonteDadosRecursoDto FonteDados { get; set; }
    }
}
