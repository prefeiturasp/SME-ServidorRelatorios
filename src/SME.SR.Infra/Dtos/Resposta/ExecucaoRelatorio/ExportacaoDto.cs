using Newtonsoft.Json;

namespace SME.SR.Infra.Dtos
{
    public class ExportacaoDto
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
    }
}
