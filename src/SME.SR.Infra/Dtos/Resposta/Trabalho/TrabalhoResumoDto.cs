using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra.Dtos
{
    public class TrabalhoResumoDto
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("version")]
        public int Versao { get; set; }

        [JsonProperty("reportUnitURI")]
        public string UnidadeRelatorioURI { get; set; }

        [JsonProperty("label")]
        public string Texto { get; set; }

        [JsonProperty("description")]
        public string Descricao { get; set; }

        [JsonProperty("owner")]
        public string Proprietario { get; set; }

        [JsonProperty("reportLabel")]
        public string TextoRelatorio { get; set; }

        [JsonProperty("state")]
        public TrabalhoEstadoDto Estado { get; set; }
    }
}
