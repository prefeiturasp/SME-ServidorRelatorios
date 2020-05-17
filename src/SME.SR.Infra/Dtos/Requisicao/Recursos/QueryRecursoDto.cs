using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra.Dtos.Requisicao.Recursos
{
   public class QueryRecursoDto : DetalhesRecursoDto
    {
        [JsonProperty("value")]
        public string Valor { get; set; }

        [JsonProperty("language")]
        public string Idioma { get; set; }

        [JsonProperty("dataSource")]
        public QueryFonteDadosRecursoDto FonteDados { get; set; }
    }
}
