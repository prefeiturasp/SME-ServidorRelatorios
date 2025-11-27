using SME.SR.Infra.CDEP;
using System;
using System.Collections.Generic;

namespace SME.SR.Infra.Dtos.Relatorios.CDEP
{
    public class FiltroRelatorioHistoricoSolicitacaoAcervo : FiltroRelatorioCdepBase
    {
        public string Solicitante { get; set; }
        public List<SituacaoSolicitacaoItem> SituacaoSolicitacao { get; set; }
        public List<TipoAcervo> TipoAcervo { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
    }
}
