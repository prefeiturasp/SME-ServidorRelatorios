using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaDto
    {
        public RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaDto()
        {
            GraficosBarras = new List<GraficoBarrasVerticalDto>();
        }

        public RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaCabecalhoDto Cabecalho { get; set; }
        public RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaPlanilhaDto Planilha { get; set;  }

        public List<GraficoBarrasVerticalDto> GraficosBarras { get; set; }
    }
}
