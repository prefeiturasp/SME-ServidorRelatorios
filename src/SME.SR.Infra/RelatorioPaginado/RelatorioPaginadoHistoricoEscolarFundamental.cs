using System.Collections.Generic;

namespace SME.SR.Infra.RelatorioPaginado
{
    public class RelatorioPaginadoHistoricoEscolarFundamental : RelatorioPaginadoHistoricoEscolarFundamentalMedio
    {
        public RelatorioPaginadoHistoricoEscolarFundamental(IEnumerable<HistoricoEscolarFundamentalDto> historicoEscolarDTOs) : base(historicoEscolarDTOs)
        {
            TabelaHistoricoTodosAnos = SecaoViewHistoricoEscolar.TabelaHistoricoTodosAnosFundamental;
            TabelaAnoAtual = SecaoViewHistoricoEscolar.TabelaAnoAtualFundamental;
        }
    }
}
