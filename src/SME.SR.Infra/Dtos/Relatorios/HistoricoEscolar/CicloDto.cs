using Newtonsoft.Json;
using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class CicloDto
    {
        [JsonProperty("nome")]
        public string Nome { get; set; }
        [JsonProperty("anos")]
        public List<int> Anos { get; set; }
    }
}
