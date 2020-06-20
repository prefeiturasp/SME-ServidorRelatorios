using Newtonsoft.Json;
using System.Collections.Generic;

namespace SME.SR.Data
{
    public class ComponenteComNotaFinal : ComponenteComNota
    {
        [JsonProperty("notaFinal")]
        public string NotaFinal { get; set; }

        [JsonProperty("notaConceitoBimestre1")]
        public string NotaConceitoBimestre1 { get; set; }

        [JsonProperty("notaConceitoBimestre2")]
        public string NotaConceitoBimestre2 { get; set; }

        [JsonProperty("notaConceitoBimestre3")]
        public string NotaConceitoBimestre3 { get; set; }

        [JsonProperty("notaConceitoBimestre4")]
        public string NotaConceitoBimestre4 { get; set; }
    }
}
