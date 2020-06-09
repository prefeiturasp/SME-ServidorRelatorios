using Newtonsoft.Json;
using System.Collections.Generic;

namespace SME.SR.Workers.SGP.Models
{
    public class RelatorioConselhoClasseBimestre : RelatorioConselhoClasseBase
    {
        [JsonProperty("listaComponentesComNota")]
        public IEnumerable<GrupoMatrizComponenteComNotaBimestre> GruposMatrizComponentesComNota { get; set; }

        [JsonProperty("listaComponenteSemNota")]
        public IEnumerable<GrupoMatrizComponenteSemNotaBimestre> GruposMatrizComponentesSemNota { get; set; }
    }
}
