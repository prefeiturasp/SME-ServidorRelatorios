using Newtonsoft.Json;
using System.Collections.Generic;

namespace SME.SR.Workers.SGP.Models
{
    public class GrupoMatrizComponenteSemNotaFinal : GrupoMatriz
    {
        [JsonProperty("componentesSemNota")]
        public IEnumerable<ComponenteSemNotaFinal> ComponentesSemNota { get; set; }
    }
}
