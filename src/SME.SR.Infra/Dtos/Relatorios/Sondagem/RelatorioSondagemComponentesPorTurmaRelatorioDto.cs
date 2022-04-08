using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioSondagemComponentesPorTurmaRelatorioDto
    {
        public RelatorioSondagemComponentesPorTurmaRelatorioDto()
        {
            GraficosBarras = new List<GraficoBarrasVerticalDto>();
        }

        public RelatorioSondagemComponentesPorTurmaCabecalhoDto Cabecalho { get; set; }
        public RelatorioSondagemComponentesPorTurmaPlanilhaDto Planilha { get; set;  }
        public List<GraficoBarrasVerticalDto> GraficosBarras { get; set; }
    }
}
