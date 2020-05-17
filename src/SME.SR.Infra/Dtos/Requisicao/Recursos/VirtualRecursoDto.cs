using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra.Dtos.Requisicao.Recursos
{
   public class VirtualRecursoDto : DetalhesRecursoDto
    {
        [JsonProperty("subDataSources")]
        public IEnumerable<SubFonteDadosRecursoDto> SubFonteDados { get; set; }
    }
}
