using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class BoletimEscolarCabecalhoDto
    {
        [JsonProperty("nomeDre")]
        public string NomeDre { get; set; }

        [JsonProperty("nomeUe")]
        public string NomeUe { get; set; }

        [JsonProperty("nomeTurma")]
        public string NomeTurma { get; set; }

        [JsonProperty("aluno")]
        public string Aluno { get; set; }

        [JsonIgnore]
        public string NomeAlunoOrdenacao { get; set; }

        [JsonProperty("codigoEol")]
        public string CodigoEol { get; set; }

        [JsonProperty("data")]
        public string Data { get; set; }
        
        [JsonProperty("frequenciaGlobal")]
        public string FrequenciaGlobal { get; set; }

        [JsonProperty("anoLetivo")]
        public string AnoLetivo { get; set; }
    }
}
