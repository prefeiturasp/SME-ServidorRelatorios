using Newtonsoft.Json;
using System.Collections.Generic;

namespace SME.SR.Infra.Dtos
{
    public class VirtualRecursoDto : DetalhesRecursoDto
    {
        [JsonProperty("subDataSources")]
        public IEnumerable<SubFonteDadosRecursoDto> SubFonteDados { get; set; }
    }
}
