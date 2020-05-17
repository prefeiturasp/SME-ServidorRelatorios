using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra.Dtos.Requisicao.Recursos
{
   public class ArquivoJRXMLRecursoDto
    {
        [JsonProperty("label")]
        public string Titulo { get; set; }

        [JsonProperty("type")]
        public string Tipo { get; set; }
    }
}
