using Newtonsoft.Json;
using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class GruposComponentesCurricularesDto
    {
        [JsonProperty("nome")]
        public string Nome { get; set; }
        
        [JsonProperty("areasDeConhecimento")]
        public List<AreaDeConhecimentoDto> AreasDeConhecimento { get; set; }

    }
}
