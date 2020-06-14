using Newtonsoft.Json;
using System.Collections.Generic;

namespace SME.SR.Data
{
    public class GrupoMatrizComponenteComNotaBimestre : GrupoMatriz
    {
        [JsonProperty("componentesComNota")]
        public IEnumerable<ComponenteComNotaBimestre> ComponentesComNota { get; set; }
    }
}
