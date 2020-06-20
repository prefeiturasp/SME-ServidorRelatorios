using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Data.Models
{
    public class ComponenteRegenciaComNotaFinal
    {
        [JsonProperty("Componente")]
        public string Componente { get; set; }

        [JsonProperty("EhEja")]
        public bool EhEja { get; set; }

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
