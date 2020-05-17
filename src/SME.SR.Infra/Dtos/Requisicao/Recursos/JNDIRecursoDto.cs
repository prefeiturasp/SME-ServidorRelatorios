using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra.Dtos.Requisicao.Recursos
{
   public class JNDIRecursoDto : DetalhesRecursoDto
    {
        [JsonProperty("jndiName")]
        public string NomeJNDI { get; set; }

        [JsonProperty("timezone")]
        public string FusoHorario { get; set; }
    }
}
