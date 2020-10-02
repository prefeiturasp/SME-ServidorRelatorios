using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class RelatorioSondagemComponentesPorTurmaPlanilhaDto
    {
        public List<RelatorioSondagemComponentesPorTurmaPlanilhaLinhasDto> Linhas;
        public RelatorioSondagemComponentesPorTurmaPlanilhaDto()
        {
            this.Linhas = new List<RelatorioSondagemComponentesPorTurmaPlanilhaLinhasDto>();
        }
    }
}
