using Newtonsoft.Json;
using System.Collections.Generic;

namespace SME.SR.Workers.SGP.Models
{
    public class GrupoMatrizComponenteComNotaFinal : GrupoMatriz
    {
        [JsonProperty("componentesComNota")]
        public IEnumerable<ComponenteComNotaFinal> ComponentesComNota { get; set; }
    }
}
