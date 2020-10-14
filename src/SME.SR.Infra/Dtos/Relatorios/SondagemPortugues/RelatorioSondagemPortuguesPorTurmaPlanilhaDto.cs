using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class RelatorioSondagemPortuguesPorTurmaPlanilhaDto
    {
        public List<RelatorioSondagemPortuguesPorTurmaPlanilhaLinhasDto> Linhas;
        public RelatorioSondagemPortuguesPorTurmaPlanilhaDto()
        {
            this.Linhas = new List<RelatorioSondagemPortuguesPorTurmaPlanilhaLinhasDto>();
        }
    }
}
