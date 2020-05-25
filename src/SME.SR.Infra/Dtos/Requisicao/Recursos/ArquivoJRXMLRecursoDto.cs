using Newtonsoft.Json;
using System.Buffers.Text;

namespace SME.SR.Infra.Dtos
{
    public class ArquivoJRXMLRecursoDto
    {
        [JsonProperty("label")]
        public string Titulo { get; set; }

        [JsonProperty("type")]
        public string Tipo { get; set; }

        [JsonProperty("content")]
        public string ConteudoBase64 { get; set; }
    }
}
