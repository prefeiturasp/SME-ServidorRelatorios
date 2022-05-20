using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class ComponenteCurricularRegenciaNotaDto
    {
        public string Codigo { get; set; }

        [JsonIgnore]
        public bool Nota { get; set; }
        public string Nome { get; set; }

        public string NotaBimestre1 { get; set; }

        public string NotaBimestre2 { get; set; }

        public string NotaBimestre3 { get; set; }

        public string NotaBimestre4 { get; set; }

        public string NotaFinal { get; set; }
    }
}
