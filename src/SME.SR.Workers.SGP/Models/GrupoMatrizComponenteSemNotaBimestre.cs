using Newtonsoft.Json;
using System.Collections.Generic;

namespace SME.SR.Workers.SGP.Models
{
    public class GrupoMatrizComponenteSemNotaBimestre : GrupoMatriz
    {
        [JsonProperty("componentesSemNota")]
        public IEnumerable<ComponenteSemNota> ComponentesSemNota { get; set; }
    }
}
