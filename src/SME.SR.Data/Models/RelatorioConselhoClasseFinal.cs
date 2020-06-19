using Newtonsoft.Json;
using System.Collections.Generic;

namespace SME.SR.Data
{
    public class RelatorioConselhoClasseFinal : RelatorioConselhoClasseBase
    {
        [JsonProperty("GruposMatrizComponentesComNota")]
        public IEnumerable<GrupoMatrizComponenteComNotaFinal> GruposMatrizComponentesComNota { get; set; }

        [JsonProperty("GruposMatrizComponentesSemNota")]
        public IEnumerable<GrupoMatrizComponenteSemNotaFinal> GruposMatrizComponentesSemNota { get; set; }
    }
}
