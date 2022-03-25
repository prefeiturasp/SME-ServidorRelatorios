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

        public string Codigo { get; set; }

        public bool Frequencia { get; set; }

        public string FrequenciaBimestre1 { get; set; }

         public string FrequenciaBimestre2 { get; set; }

         public string FrequenciaBimestre3 { get; set; }

        public string FrequenciaBimestre4 { get; set; }
        public string FrequenciaFinal { get; set; }
        public List<ComponenteCurricularRegenciaNotaDto> ComponentesCurriculares { get; set; }
    }
}
