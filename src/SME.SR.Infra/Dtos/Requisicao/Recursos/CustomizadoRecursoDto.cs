using Newtonsoft.Json;
using System.Collections.Generic;

namespace SME.SR.Infra.Dtos
{
    public class CustomizadoRecursoDto : DetalhesRecursoDto
    {
        [JsonProperty("serviceClass")]
        public string ClasseServico { get; set; }

        [JsonProperty("dataSourceName")]
        public string NomeFonteDados { get; set; }

        [JsonProperty("properties")]
        public IEnumerable<PropriedadesRecursoDto> Propriedades { get; set; }
    }
}
