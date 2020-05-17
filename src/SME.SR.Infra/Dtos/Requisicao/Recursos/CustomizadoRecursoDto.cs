using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra.Dtos.Requisicao.Recursos
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
