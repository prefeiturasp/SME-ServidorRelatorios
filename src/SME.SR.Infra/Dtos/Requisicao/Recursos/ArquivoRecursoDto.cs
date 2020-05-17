using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra.Dtos.Requisicao.Recursos
{
    public class ArquivoRecursoDto : DetalhesRecursoDto
    {
        [JsonProperty("content")]
        public string Conteudo { get; set; }
    }
}
