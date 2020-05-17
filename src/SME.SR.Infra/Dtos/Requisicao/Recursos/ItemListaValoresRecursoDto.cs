using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra.Dtos.Requisicao.Recursos
{
    public class ItemListaValoresRecursoDto
    {
        [JsonProperty("label")]
        public string Titulo { get; set; }

        [JsonProperty("value")]
        public string Valor { get; set; }
    }
}
