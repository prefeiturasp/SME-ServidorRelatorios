using Newtonsoft.Json;

namespace SME.SR.Infra.Dtos
{
    public class ParametroRepositorioRecursoDto
    {
        [JsonProperty("name")]
        public string Nome { get; set; }

        [JsonProperty("value")]
        public string[] Valores { get; set; }
    }
}
