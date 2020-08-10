using Newtonsoft.Json;
using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class GruposComponentesCurricularesEJADto
    {
        [JsonProperty("nome")]
        public string Nome { get; set; }

        [JsonProperty("areasDeConhecimento")]
        public List<AreaDeConhecimentoEJADto> AreasDeConhecimento { get; set; }
    }
}
