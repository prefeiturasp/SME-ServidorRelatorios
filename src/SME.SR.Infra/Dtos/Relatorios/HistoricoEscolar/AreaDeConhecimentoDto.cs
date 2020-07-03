using Newtonsoft.Json;
using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class AreaDeConhecimentoDto
    {
        [JsonProperty("nome")]
        public string Nome { get; set; }

        [JsonProperty("componentesCurriculares")]
        public List<ComponenteCurricularDto> ComponentesCurriculares { get; set; }
    }
}
