using Newtonsoft.Json;

namespace SME.SR.Infra.Dtos
{
    public class BeanRecursoDto : DetalhesRecursoDto
    {
        [JsonProperty("beanName")]
        public string NomeBean { get; set; }

        [JsonProperty("beanMethod")]
        public string MetodoBean { get; set; }
    }
}
