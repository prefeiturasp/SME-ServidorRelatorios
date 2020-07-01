using Newtonsoft.Json;
using System.Collections.Generic;

namespace SME.SR.Infra.Dtos.Relatorios.HistoricoEscolar
{
    public class CicloDto
    {
        [JsonProperty("nome")]
        public string nome { get; set; }
        [JsonProperty("anos")]
        public List<int> anos { get; set; }
    }
}
