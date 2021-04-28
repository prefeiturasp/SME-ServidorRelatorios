using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace SME.SR.Infra
{
    public class GruposComponentesCurricularesTransferenciaDto
    {
        [JsonProperty("nome")]
        public string Nome { get; set; }

        [JsonProperty("areasDeConhecimento")]
        public List<AreaDeConhecimentoTransferenciaDto> AreasDeConhecimento { get; set; }

        [JsonIgnore]
        public GruposComponentesCurricularesTransferenciaDto ObterAreasComNotaValida
        {
            get
            {
                AreasDeConhecimento = AreasDeConhecimento.Where(ac => ac.ComponentesCurriculares.Any())?.ToList();
                return this;
            }
        }
    }
}
