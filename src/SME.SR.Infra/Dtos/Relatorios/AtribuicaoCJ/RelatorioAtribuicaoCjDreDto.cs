using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioAtribuicaoCjDreDto
    {
        public RelatorioAtribuicaoCjDreDto()
        {
            Ues = new List<RelatorioAtribuicaoCjUeDto>();
        }
        public string Nome { get; set; }
        public List<RelatorioAtribuicaoCjUeDto> Ues { get; set; }
    }
}