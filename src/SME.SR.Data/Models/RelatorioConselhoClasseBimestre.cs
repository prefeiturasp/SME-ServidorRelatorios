using Newtonsoft.Json;
using System.Collections.Generic;

namespace SME.SR.Data
{
    public class RelatorioConselhoClasseBimestre : RelatorioConselhoClasseBase
    {
        [JsonProperty("gruposMatrizComponentesComNota")]
        public IEnumerable<GrupoMatrizComponenteComNotaBimestre> GruposMatrizComponentesComNota { get; set; }

        [JsonProperty("gruposMatrizComponentesSemNota")]
        public IEnumerable<GrupoMatrizComponenteSemNotaBimestre> GruposMatrizComponentesSemNota { get; set; }
    }
}
