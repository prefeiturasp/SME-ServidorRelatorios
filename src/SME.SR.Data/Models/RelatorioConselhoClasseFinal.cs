using Newtonsoft.Json;
using System.Collections.Generic;

namespace SME.SR.Data
{
    public class RelatorioConselhoClasseFinal : RelatorioConselhoClasseBase
    {
        [JsonProperty("listaComponentesComNota")]
        public IEnumerable<GrupoMatrizComponenteComNotaFinal> GruposMatrizComponentesComNota { get; set; }

        [JsonProperty("listaComponenteSemNota")]
        public IEnumerable<GrupoMatrizComponenteSemNotaFinal> GruposMatrizComponentesSemNota { get; set; }
    }
}
