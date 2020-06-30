using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class ComponenteCurricularRegenciaNotaDto
    {
        [JsonProperty("nome")]
        public string Nome { get; set; }

        [JsonProperty("notaBimestre1")]
        public string NotaBimestre1 { get; set; }

        [JsonProperty("notaBimestre2")]
        public string NotaBimestre2 { get; set; }

        [JsonProperty("notaBimestre3")]
        public string NotaBimestre3 { get; set; }

        [JsonProperty("notaBimestre4")]
        public string NotaBimestre4 { get; set; }

        [JsonProperty("notaFinal")]
        public string NotaFinal { get; set; }
    }
}
