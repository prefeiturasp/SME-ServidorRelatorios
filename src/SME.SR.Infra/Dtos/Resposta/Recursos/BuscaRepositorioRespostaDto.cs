using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra.Dtos
{
   public class BuscaRepositorioRespostaDto
    {
        [JsonProperty("resourceLookup")]
        public IEnumerable<DetalhesRecursoDto> DetalhesRecursos { get; set; }
    }
}
