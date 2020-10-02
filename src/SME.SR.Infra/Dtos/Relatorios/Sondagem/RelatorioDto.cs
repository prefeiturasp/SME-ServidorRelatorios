using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra.Dtos.Relatorios.Sondagem
{
    public class RelatorioDto
    {
        [JsonProperty("componente")]
        public ComponenteCurricularDto Componente { get; set; }

        [JsonProperty("aluno")]
        public AlunoSituacaoDto AlunoSituacao { get; set; }

        [JsonProperty("ordens")]
        public List<OrdemDto> Ordens { get; set; } = new List<OrdemDto>();
    }
}
