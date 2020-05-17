using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra.Dtos.Requisicao.Recursos
{
    public class PropriedadesRecursoDto
    {
        [JsonProperty("key")]
        public string Chave { get; set; }

        [JsonProperty("value")]
        public string Valor { get; set; }
    }
}
