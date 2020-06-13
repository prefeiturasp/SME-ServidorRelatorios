using Newtonsoft.Json;
using System.Collections.Generic;

namespace SME.SR.Data
{
    public class ComponenteComNotaFinal : ComponenteComNota
    {
        [JsonProperty("notaFinal")]
        public string NotaFinal { get; set; }

        [JsonProperty("notasBimestre")]
        public IEnumerable<NotaBimestre> NotasBimestre { get; set; }


    }
}
