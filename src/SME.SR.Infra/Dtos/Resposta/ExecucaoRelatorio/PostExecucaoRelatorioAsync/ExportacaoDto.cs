using Newtonsoft.Json;
using System;

namespace SME.SR.Infra.Dtos.Resposta
{
    public class ExportacaoDto
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
    }
}
