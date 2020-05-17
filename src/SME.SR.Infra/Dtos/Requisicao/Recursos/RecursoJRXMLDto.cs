using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra.Dtos.Requisicao.Recursos
{
    public class RecursoJRXMLDto
    {
        [JsonProperty("resource")]
        public RecursoDto Recurso { get; set; }
    }
}
