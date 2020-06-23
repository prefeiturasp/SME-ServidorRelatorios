using Newtonsoft.Json;
using System.Collections.Generic;

namespace SME.SR.Data
{
    public class GrupoMatrizComponenteSemNotaFinal : GrupoMatriz
    {
        [JsonProperty("ComponentesSemNota")]
        public IEnumerable<ComponenteSemNotaFinal> ComponentesSemNota { get; set; }
    }
}
