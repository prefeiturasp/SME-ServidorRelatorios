using Newtonsoft.Json;
using System.Collections.Generic;

namespace SME.SR.Data
{
    public class GrupoMatrizComponenteSemNotaBimestre : GrupoMatriz
    {
        [JsonProperty("componentesSemNota")]
        public IEnumerable<ComponenteSemNota> ComponentesSemNota { get; set; }
    }
}
