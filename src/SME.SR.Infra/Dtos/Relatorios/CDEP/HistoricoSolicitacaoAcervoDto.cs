using SME.SR.Infra.CDEP;
using System;

namespace SME.SR.Infra.Dtos.Relatorios.CDEP
{
    public class HistoricoSolicitacaoAcervoDto
    {
        public string NomeSolicitante { get; set; }
        public string CodigoTombo { get; set; }
        public DateTime DataSolicitacao { get; set; }
        public DateTime? DataVisita { get; set; }
        public TipoAcervo TipoAcervo { get; set; }
        public SituacaoSolicitacaoItem SituacaoSolicitacao { get; set; }
        public string Titulo { get; set; }
    }
}
