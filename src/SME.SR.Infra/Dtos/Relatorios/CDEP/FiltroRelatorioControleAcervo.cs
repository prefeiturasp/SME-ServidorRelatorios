using SME.SR.Infra.CDEP;

namespace SME.SR.Infra.Dtos.Relatorios.CDEP
{
    public class FiltroRelatorioControleAcervo
    {
        public string Usuario { get; set; }
        public string UsuarioRF { get; set; }
        public SituacaoAcervo SituacaoAcervo { get; set; }
        public TipoAcervo TipoAcervo { get; set; }
        public long[] TiposAcervosPermitidos { get; set; }
    }
}
