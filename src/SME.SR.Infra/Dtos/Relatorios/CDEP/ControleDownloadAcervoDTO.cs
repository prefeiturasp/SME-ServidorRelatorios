using SME.SR.Infra.CDEP;

namespace SME.SR.Infra.Dtos.Relatorios.CDEP
{
    public class ControleDownloadAcervoDTO
    {
        public TipoAcervo TipoAcervo { get; set; }
        public int QuantidadeVezBaixado { get; set; }
        public string Titulo { get; set; }
        public string CodigoTombo { get; set; }
    }
}
