using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class GrupoMatrizComponenteCurricularDto
    {
        [JsonProperty("id")]
        public int Id{ get; set; }

        [JsonProperty("nome")]
        public string Nome { get; set; }

        [JsonProperty("descricao")]
        public string Descricao { get; set; }

        [JsonProperty("componentesCurriculares")]
        public List<ComponenteCurricularDto> ComponentesCurriculares { get; set; }
    }
}
