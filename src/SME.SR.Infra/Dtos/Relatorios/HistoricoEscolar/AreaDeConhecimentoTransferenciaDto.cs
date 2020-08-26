using Newtonsoft.Json;
using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class AreaDeConhecimentoTransferenciaDto
    {
        [JsonProperty("nome")]
        public string Nome { get; set; }

        [JsonProperty("componentesCurriculares")]
        public List<ComponenteCurricularHistoricoEscolarTransferenciaDto> ComponentesCurriculares { get; set; }
    }
}
