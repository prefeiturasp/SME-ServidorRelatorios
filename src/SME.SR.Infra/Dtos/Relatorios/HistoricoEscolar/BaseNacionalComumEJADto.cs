using Newtonsoft.Json;
using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class BaseNacionalComumEJADto
    {
        [JsonProperty("areasDeConhecimento")] public List<AreaDeConhecimentoEJADto> AreasDeConhecimento { get; set; }
    }
}
