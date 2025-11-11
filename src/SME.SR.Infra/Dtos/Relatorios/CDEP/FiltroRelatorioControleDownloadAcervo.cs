using SME.SR.Infra.CDEP;

namespace SME.SR.Infra.Dtos.Relatorios.CDEP
{
    public class FiltroRelatorioControleDownloadAcervo
    {
        public string Usuario { get; set; }
        public string UsuarioRF { get; set; }
        public string Titulo { get; set; }
        public TipoAcervo TipoAcervo { get; set; }
    }
}
