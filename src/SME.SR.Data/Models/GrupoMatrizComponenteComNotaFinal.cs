﻿using Newtonsoft.Json;
using SME.SR.Data.Models;
using System.Collections.Generic;

namespace SME.SR.Data
{
    public class GrupoMatrizComponenteComNotaFinal : GrupoMatriz
    {
        [JsonProperty("TipoNota")]
        public string TipoNota { get; set; }

        [JsonProperty("ComponentesComNota")]
        public IEnumerable<ComponenteComNotaFinal> ComponentesComNota { get; set; }

        [JsonProperty("ComponenteComNotaRegencia")]
        public ComponenteFrequenciaRegenciaFinal ComponentesComNotaRegencia { get; set; }
    }
}
