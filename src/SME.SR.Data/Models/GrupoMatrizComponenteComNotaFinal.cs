using Newtonsoft.Json;
using System.Collections.Generic;

namespace SME.SR.Data
{
    public class GrupoMatrizComponenteComNotaFinal : GrupoMatriz
    {
        [JsonProperty("ComponentesComNota")]
        public IEnumerable<ComponenteComNotaFinal> ComponentesComNota { get; set; }
    }
}
