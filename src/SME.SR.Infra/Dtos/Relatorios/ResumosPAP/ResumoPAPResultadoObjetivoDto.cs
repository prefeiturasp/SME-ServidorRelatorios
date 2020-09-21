using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class ResumoPAPResultadoObjetivoDto
    {
        public IEnumerable<ResumoPAPResultadoAnoDto> Anos { get; set; }
        public string ObjetivoDescricao { get; set; }
        public IEnumerable<ResumoPAPResultadoRespostaDto> Total { get; set; }
    }
}
