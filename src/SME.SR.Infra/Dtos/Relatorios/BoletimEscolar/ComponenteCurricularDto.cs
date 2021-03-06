﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace SME.SR.Infra
{
    public class ComponenteCurricularDto
    {
        public ComponenteCurricularDto()
        {
            NotaBimestre1 = "-";
            NotaBimestre2 = "-";
            NotaBimestre3 = "-";
            NotaBimestre4 = "-";
        }
        [JsonIgnore]
        public string Codigo { get; set; }

        [JsonIgnore]
        public bool Nota { get; set; }

        [JsonIgnore]
        public bool Frequencia { get; set; }

        [JsonProperty("nome")]
        public string Nome { get; set; }

        [JsonProperty("notaBimestre1")]
        public string NotaBimestre1 { get; set; }

        [JsonProperty("frequenciaBimestre1")]
        public string FrequenciaBimestre1 { get; set; }

        [JsonProperty("notaBimestre2")]
        public string NotaBimestre2 { get; set; }

        [JsonProperty("frequenciaBimestre2")]
        public string FrequenciaBimestre2 { get; set; }

        [JsonProperty("notaBimestre3")]
        public string NotaBimestre3 { get; set; }

        [JsonProperty("frequenciaBimestre3")]
        public string FrequenciaBimestre3 { get; set; }

        [JsonProperty("notaBimestre4")]
        public string NotaBimestre4 { get; set; }

        [JsonProperty("frequenciaBimestre4")]
        public string FrequenciaBimestre4 { get; set; }

        [JsonProperty("notaFinal")]
        public string NotaFinal { get; set; }

        [JsonProperty("frequenciaFinal")]
        public string FrequenciaFinal { get; set; }
    }
}
