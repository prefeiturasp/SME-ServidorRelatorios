using Newtonsoft.Json;
using System.Collections.Generic;

namespace SME.SR.Data
{
    public class GrupoMatrizComponenteSemNotaFinal : GrupoMatriz
    {
        [JsonProperty("componentesSemNota")]
        public IEnumerable<ComponenteSemNotaFinal> ComponentesSemNota { get; set; }
    }
}
