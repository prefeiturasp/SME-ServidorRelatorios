using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Data.Models
{
    public class ComponenteFrequenciaRegenciaFinal : ComponenteRegenciaComNota
    {
        public ComponenteFrequenciaRegenciaFinal()
        {
            ComponentesCurriculares = new List<ComponenteRegenciaComNotaFinal>();
        }

        [JsonProperty("Aulas")]
        public int? Aulas { get; set; }

        [JsonProperty("ComponentesCurriculares")]
        public List<ComponenteRegenciaComNotaFinal> ComponentesCurriculares { get; set; }

    }
}
