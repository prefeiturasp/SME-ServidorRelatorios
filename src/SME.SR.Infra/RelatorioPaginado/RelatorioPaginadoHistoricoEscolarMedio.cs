using System.Collections.Generic;

namespace SME.SR.Infra.RelatorioPaginado
{
    public class RelatorioPaginadoHistoricoEscolarMedio : RelatorioPaginadoHistoricoEscolarFundamentalMedio
    {
        public RelatorioPaginadoHistoricoEscolarMedio(IEnumerable<HistoricoEscolarFundamentalDto> historicoEscolarDTOs) : base(historicoEscolarDTOs)
        {
            TabelaHistoricoTodosAnos = SecaoViewHistoricoEscolar.TabelaHistoricoTodosAnosMedio;
            TabelaAnoAtual = SecaoViewHistoricoEscolar.TabelaAnoAtualFundamental;
        }
    }
}
