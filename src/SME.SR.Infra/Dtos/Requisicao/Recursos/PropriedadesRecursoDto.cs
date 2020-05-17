using Newtonsoft.Json;

namespace SME.SR.Infra.Dtos
{
    public class PropriedadesRecursoDto
    {
        [JsonProperty("key")]
        public string Chave { get; set; }

        [JsonProperty("value")]
        public string Valor { get; set; }
    }
}
