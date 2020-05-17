using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra.Dtos.Requisicao.Recursos
{
    public class BeanRecursoDto : DetalhesRecursoDto
    {
        [JsonProperty("beanName")]
        public string NomeBean { get; set; }

        [JsonProperty("beanMethod")]
        public string MetodoBean { get; set; }
    }
}
