using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioAcompanhamentoFechamentoConsolidadoPorUeDto
    {
        public RelatorioAcompanhamentoFechamentoConsolidadoPorUeDto()
        {
            Ues =  new List<RelatorioAcompanhamentoFechamentoUesDto>();
        }

        public RelatorioAcompanhamentoFechamentoCabecalhoDto Cabecalho { get; set; }
        public List<RelatorioAcompanhamentoFechamentoUesDto> Ues { get; set; }
    }
}
