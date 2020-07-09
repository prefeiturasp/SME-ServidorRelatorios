using System.Collections.Generic;
using Newtonsoft.Json;

namespace SME.SR.Infra
{
    public class BaseNacionalComumDto
    {
        [JsonProperty("areasDeConhecimento")] public List<AreaDeConhecimentoDto> AreasDeConhecimento { get; set; }
    }
}