using Newtonsoft.Json;

namespace SME.SR.Infra.Dtos
{
    public class ArquivoJRXMLRecursoDto
    {
        [JsonProperty("label")]
        public string Titulo { get; set; }

        [JsonProperty("type")]
        public string Tipo { get; set; }
    }
}
