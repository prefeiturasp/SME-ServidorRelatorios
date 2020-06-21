using Newtonsoft.Json;
using System.Collections.Generic;

namespace SME.SR.Data
{
    public class RelatorioConselhoClasseArray
    {
        public RelatorioConselhoClasseArray()
        {
            Relatorio = new List<RelatorioConselhoClasseBase>();
        }
        [JsonProperty("RelatorioConselhoDeClasse")]
        public List<RelatorioConselhoClasseBase> Relatorio { get; set; }
    }
}
