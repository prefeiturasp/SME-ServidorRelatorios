using Newtonsoft.Json;
using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class AreaConhecimentoComponenteCurricularDto
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("grupoMatrizId")]
        public int GrupoMatrizId { get; set; }

        [JsonProperty("nome")]
        public string Nome { get; set; }

        [JsonProperty("descricao")]
        public string Descricao { get; set; }

        [JsonProperty("componentesCurriculares")]
        public List<ComponenteCurricularDto> ComponentesCurriculares { get; set; }
    }
}
