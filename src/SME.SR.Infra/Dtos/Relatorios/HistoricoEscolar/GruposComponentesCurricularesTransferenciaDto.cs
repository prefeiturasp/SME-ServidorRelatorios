using Newtonsoft.Json;
using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class GruposComponentesCurricularesTransferenciaDto
    {
        [JsonProperty("nome")]
        public string Nome { get; set; }

        [JsonProperty("areasDeConhecimento")]
        public List<AreaDeConhecimentoTransferenciaDto> AreasDeConhecimento { get; set; }
    }
}
