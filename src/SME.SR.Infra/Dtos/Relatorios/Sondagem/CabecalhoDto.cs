using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra.Dtos.Relatorios.Sondagem
{
    public class CabecalhoDto
    {
        [JsonProperty("dre")]
        public string Dre { get; set; }

        [JsonProperty("ue")]
        public string Ue { get; set; }

        [JsonProperty("atoLetivo")]
        public int AnoLetivo { get; set; }

        [JsonProperty("ano")]
        public string Ano { get; set; }

        [JsonProperty("turma")]
        public string Turma { get; set; }

        [JsonProperty("componenteCurricular")]
        public string ComponenteCurricular { get; set; }

        [JsonProperty("proficiencia")]
        public string Proficiencia { get; set; }

        [JsonProperty("periodo")]
        public string Periodo { get; set; }

        [JsonProperty("usuario")]
        public string Usuario { get; set; }

        [JsonProperty("rf")]
        public string Rf { get; set; }

        [JsonProperty("dataSolicitacao")]
        public DateTime DataSolicitacao { get; set; }
    }
}
