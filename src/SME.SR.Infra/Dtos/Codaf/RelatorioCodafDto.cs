using System.Collections.Generic;

namespace SME.SR.Infra.Dtos.Codaf
{
    public class RelatorioCodafDto
    {
        public List<TurmaRelatorioCodafDto> Turmas { get; set; } = new List<TurmaRelatorioCodafDto>();
    }
}