using System.Collections.Generic;

namespace SME.SR.Infra  
{
    public class ResumoPAPResultadoAnoDto
    {
        public string AnoDescricao { get; set; }
        public IEnumerable<ResumoPAPResultadoRespostaDto> Respostas { get; set; }
    }
}
