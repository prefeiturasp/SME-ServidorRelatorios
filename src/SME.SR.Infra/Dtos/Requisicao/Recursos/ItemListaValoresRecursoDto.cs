using Newtonsoft.Json;

namespace SME.SR.Infra.Dtos
{
    public class ItemListaValoresRecursoDto
    {
        [JsonProperty("label")]
        public string Titulo { get; set; }

        [JsonProperty("value")]
        public string Valor { get; set; }
    }
}
