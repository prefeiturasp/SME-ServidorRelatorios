using Newtonsoft.Json;
using System.Collections.Generic;

namespace SME.SR.Infra.Dtos.Relatorios.HistoricoEscolar
{
    public class GruposComponentesCurricularesDto
    {
        [JsonProperty("nome")]
        public string nome { get; set; }
        
        [JsonProperty("areasDeConhecimento")]
        public List<AreaDeConhecimentoDto> areasDeConhecimento { get; set; }

    }
}
