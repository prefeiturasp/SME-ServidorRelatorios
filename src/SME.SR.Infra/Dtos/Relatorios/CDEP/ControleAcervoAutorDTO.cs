using SME.SR.Infra.CDEP;

namespace SME.SR.Infra.Dtos.Relatorios.CDEP
{
    public class ControleAcervoAutorDTO
    {
        public string Autor { get; set; }
        public string Tombo { get; set; }
        public string Titulo { get; set; }
        public TipoAcervo TipoAcervo { get; set; }
    }
}
