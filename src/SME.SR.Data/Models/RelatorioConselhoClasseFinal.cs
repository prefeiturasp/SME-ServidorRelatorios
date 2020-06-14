using Newtonsoft.Json;
using System.Collections.Generic;

namespace SME.SR.Data
{
    public class RelatorioConselhoClasseFinal : RelatorioConselhoClasseBase
    {
        [JsonProperty("ListaComponentesComNota")]
        public IEnumerable<GrupoMatrizComponenteComNotaFinal> GruposMatrizComponentesComNota { get; set; }

        [JsonProperty("ListaComponenteSemNota")]
        public IEnumerable<GrupoMatrizComponenteSemNotaFinal> GruposMatrizComponentesSemNota { get; set; }
    }
}
