using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra.Dtos.Relatorios.Sondagem
{
    public class RelatorioDto
    {
        [JsonProperty("componenteCurricular")]
        public ComponenteCurricularDto ComponenteCurricular { get; set; }

        [JsonProperty("aluno")]
        public AlunoDto Aluno { get; set; }

        [JsonProperty("ordens")]
        public List<OrdemDto> Ordens { get; set; } = new List<OrdemDto>();
    }
}
