using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace SME.SR.Infra
{
    public class ComponenteCurricularDto
    {
        [JsonIgnore]
        public string Codigo { get; set; }
        public string CodigoComponenteCurricularTerritorioSaber { get; set; }

        public bool Nota { get; set; }

        public bool Frequencia { get; set; }

        public string Nome { get; set; }

        public string NotaBimestre1 { get; set; }

        public string FrequenciaBimestre1 { get; set; }

        public string NotaBimestre2 { get; set; }

        public string FrequenciaBimestre2 { get; set; }
        public string NotaBimestre3 { get; set; }

        public string FrequenciaBimestre3 { get; set; }

        public string NotaBimestre4 { get; set; }

        public string FrequenciaBimestre4 { get; set; }

        public string NotaFinal { get; set; }

        public string FrequenciaFinal { get; set; }
        public long Grupo { get; set; }
        public string Professor { get; set; }
        public bool TerritorioSaber { get; set; }
    }
}