using Newtonsoft.Json;
using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class AreaDeConhecimentoDto
    {
        [JsonProperty("nome")]
        public string Nome { get; set; }

        [JsonProperty("componentesCurriculares")]
        public List<ComponenteCurricularHistoricoEscolarDto> ComponentesCurriculares { get; set; }
    }
}
