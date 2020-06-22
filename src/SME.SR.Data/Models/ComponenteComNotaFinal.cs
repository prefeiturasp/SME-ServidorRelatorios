using Newtonsoft.Json;
using System.Collections.Generic;

namespace SME.SR.Data
{
    public class ComponenteComNotaFinal : ComponenteComNota
    {
        [JsonProperty("NotaFinal")]
        public string NotaFinal { get; set; }

        [JsonProperty("NotaConceitoBimestre1")]
        public string NotaConceitoBimestre1 { get; set; }

        [JsonProperty("NotaConceitoBimestre2")]
        public string NotaConceitoBimestre2 { get; set; }

        [JsonProperty("NotaConceitoBimestre3")]
        public string NotaConceitoBimestre3 { get; set; }

        [JsonProperty("NotaConceitoBimestre4")]
        public string NotaConceitoBimestre4 { get; set; }
    }
}
