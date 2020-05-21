using Newtonsoft.Json;

namespace SME.SR.Infra.Dtos
{
    public class SubFonteDadosRecursoDto
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("uri")]
        public string Caminho { get; set; }
    }
}
