using Newtonsoft.Json;
using System.Collections.Generic;

namespace SME.SR.Data
{
    public class ComponenteComNotaFinal : ComponenteComNota
    {
        [JsonProperty("NotaFinal")]
        public string NotaFinal { get; set; }

        [JsonProperty("NotasBimestre")]
        public IEnumerable<NotaBimestre> NotasBimestre { get; set; }


    }
}
