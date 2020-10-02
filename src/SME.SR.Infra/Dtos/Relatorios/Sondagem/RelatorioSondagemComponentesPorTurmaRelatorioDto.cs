using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class RelatorioSondagemComponentesPorTurmaRelatorioDto
    {
        public CabecalhoDto Cabecalho { get; set; }
        public RelatorioSondagemComponentesPorTurmaPlanilhaDto Planilha { get; set;  } 
    }
}
