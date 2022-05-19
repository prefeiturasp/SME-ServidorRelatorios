using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioSondagemComponentesPorTurmaPlanilhaDto
    {
        public List<RelatorioSondagemComponentesPorTurmaPlanilhaLinhasDto> Linhas;
        public RelatorioSondagemComponentesPorTurmaPlanilhaDto()
        {
            Linhas = new List<RelatorioSondagemComponentesPorTurmaPlanilhaLinhasDto>();
        }
    }
}
