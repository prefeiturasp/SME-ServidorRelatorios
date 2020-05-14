using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra.Dtos.Resposta.ControlesEntrada
{
    public class EstadoDto
    {
        [JsonProperty("uri")]
        public string Uri { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("value")]
        public object Valor { get; set; }

        [JsonProperty("options")]
        public IList<OpcaoDto> Opcoes { get; set; }
    }
}
