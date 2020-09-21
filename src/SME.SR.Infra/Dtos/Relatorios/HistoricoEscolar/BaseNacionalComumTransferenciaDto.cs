using Newtonsoft.Json;
using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class BaseNacionalComumTransferenciaDto
    {
        [JsonProperty("areasDeConhecimento")] public List<AreaDeConhecimentoTransferenciaDto> AreasDeConhecimento { get; set; }
    }
}
