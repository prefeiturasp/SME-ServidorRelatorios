using Newtonsoft.Json;

namespace SME.SR.Infra.Dtos
{
    public class JNDIRecursoDto : DetalhesRecursoDto
    {
        [JsonProperty("jndiName")]
        public string NomeJNDI { get; set; }

        [JsonProperty("timezone")]
        public string FusoHorario { get; set; }
    }
}
