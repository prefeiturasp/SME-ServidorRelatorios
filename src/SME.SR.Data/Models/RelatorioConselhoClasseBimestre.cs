using Newtonsoft.Json;
using System.Collections.Generic;

namespace SME.SR.Data
{
    public class RelatorioConselhoClasseBimestre : RelatorioConselhoClasseBase
    {
        [JsonProperty("GruposMatrizComponentesComNota")]
        public IEnumerable<GrupoMatrizComponenteComNotaBimestre> GruposMatrizComponentesComNota { get; set; }

        [JsonProperty("GruposMatrizComponentesSemNota")]
        public IEnumerable<GrupoMatrizComponenteSemNotaBimestre> GruposMatrizComponentesSemNota { get; set; }
    }
}
