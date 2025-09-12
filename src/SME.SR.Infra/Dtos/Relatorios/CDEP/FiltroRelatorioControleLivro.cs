using SME.SR.Infra.CDEP;
using System.Collections.Generic;

namespace SME.SR.Infra.Dtos.Relatorios.CDEP
{
    public class FiltroRelatorioControleLivro
    {
        public string Solicitante { get; set; }
        public string Tombo { get; set; }
        public string Usuario { get; set; }
        public string UsuarioRF { get; set; }
        public SituacaoSolicitacaoItem SituacaoSolicitacaoItem { get; set; }
        public List<SituacaoEmprestimo>? SituacaoEmprestimo { get; set; }
        public ModeloRelatorio Modelo { get; set; }
        public bool SomenteDevolvidos { get; set; }
        public long[] TiposAcervosPermitidos { get; set; }
    }
}
