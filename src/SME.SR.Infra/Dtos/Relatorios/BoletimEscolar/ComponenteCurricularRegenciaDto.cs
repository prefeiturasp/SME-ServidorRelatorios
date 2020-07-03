using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class ComponenteCurricularRegenciaDto
    {
        public ComponenteCurricularRegenciaDto()
        {
            ComponentesCurriculares = new List<ComponenteCurricularRegenciaNotaDto>();
        }

        [JsonIgnore]
        public string Codigo { get; set; }

        [JsonProperty("frequenciaBimestre1")]
        public string FrequenciaBimestre1 { get; set; }

        [JsonProperty("frequenciaBimestre2")]
        public string FrequenciaBimestre2 { get; set; }

        [JsonProperty("frequenciaBimestre3")]
        public string FrequenciaBimestre3 { get; set; }

        [JsonProperty("frequenciaBimestre4")]
        public string FrequenciaBimestre4 { get; set; }

        [JsonProperty("frequenciaFinal")]
        public string FrequenciaFinal { get; set; }

        [JsonProperty("componentesCurriculares")]
        public List<ComponenteCurricularRegenciaNotaDto> ComponentesCurriculares { get; set; }
    }
}
