using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra.Dtos.Requisicao.Recursos
{
    public class QueryFonteDadosReferenciaRecursoDto
    {
        [JsonProperty("uri")]
        public string Caminho { get; set; }
    }
}
