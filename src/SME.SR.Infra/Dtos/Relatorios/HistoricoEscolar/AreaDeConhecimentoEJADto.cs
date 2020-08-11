using Newtonsoft.Json;
using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class AreaDeConhecimentoEJADto
    {
        [JsonProperty("nome")]
        public string Nome { get; set; }

        [JsonProperty("componentesCurriculares")]
        public List<ComponenteCurricularHistoricoEscolarEJADto> ComponentesCurriculares { get; set; }
    }
}
