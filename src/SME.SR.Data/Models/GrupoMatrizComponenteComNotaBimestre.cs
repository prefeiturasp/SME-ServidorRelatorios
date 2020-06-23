using Newtonsoft.Json;
using System.Collections.Generic;

namespace SME.SR.Data
{
    public class GrupoMatrizComponenteComNotaBimestre : GrupoMatriz
    {
        [JsonProperty("TipoNota")]
        public string TipoNota { get; set; }

        [JsonProperty("ComponentesComNota")]
        public IEnumerable<ComponenteComNotaBimestre> ComponentesComNota { get; set; }

        [JsonProperty("ComponenteComNotaRegencia")]
        public ComponenteFrequenciaRegenciaBimestre ComponenteComNotaRegencia { get; set; }

    }
}
