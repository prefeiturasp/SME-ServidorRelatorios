using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class ResumoPAPTotalResultadoDto
    {
        public string EixoDescricao { get; set; }
        public IEnumerable<ResumoPAPResultadoObjetivoDto> Objetivos { get; set; }
    }
}
