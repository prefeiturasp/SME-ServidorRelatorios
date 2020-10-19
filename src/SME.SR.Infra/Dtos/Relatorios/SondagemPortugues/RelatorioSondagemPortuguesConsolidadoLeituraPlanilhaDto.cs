using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class RelatorioSondagemPortuguesConsolidadoLeituraPlanilhaDto
    {
        public List<RelatorioSondagemPortuguesConsolidadoLeituraPlanilhaLinhaDto> Linhas;
        public RelatorioSondagemPortuguesConsolidadoLeituraPlanilhaDto()
        {
            Linhas = new List<RelatorioSondagemPortuguesConsolidadoLeituraPlanilhaLinhaDto>();
        }
    }
}
