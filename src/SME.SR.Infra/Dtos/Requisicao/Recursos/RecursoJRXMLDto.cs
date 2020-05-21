using Newtonsoft.Json;

namespace SME.SR.Infra.Dtos
{
    public class RecursoJRXMLDto
    {
        [JsonProperty("resource")]
        public RecursoDto Recurso { get; set; }
    }
}
