using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Data
{
    public class ComponenteFrequenciaRegenciaBimestre : ComponenteComNota
    {
        public ComponenteFrequenciaRegenciaBimestre()
        {
            ComponentesCurriculares = new List<ComponenteRegenciaComNotaBimestre>();
        }

        [JsonProperty("Aulas")]
        public int? Aulas { get; set; }

        [JsonProperty("ComponentesCurriculares")]
        public List<ComponenteRegenciaComNotaBimestre> ComponentesCurriculares { get; set; }
    }
}
