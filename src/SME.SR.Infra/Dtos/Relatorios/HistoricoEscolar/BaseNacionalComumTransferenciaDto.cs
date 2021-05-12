using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace SME.SR.Infra
{
    public class BaseNacionalComumTransferenciaDto
    {
        [JsonProperty("areasDeConhecimento")] public List<AreaDeConhecimentoTransferenciaDto> AreasDeConhecimento { get; set; }

        [JsonIgnore]
        public BaseNacionalComumTransferenciaDto ObterAreasComNotaValida
        {
            get
            {
                AreasDeConhecimento = AreasDeConhecimento.Where(ac => ac.ComponentesCurriculares.Any())?.ToList();
                return this;
            }
        }
    }
}
