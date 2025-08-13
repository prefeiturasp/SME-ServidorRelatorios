using SME.SR.Infra.CDEP;
using System.Collections.Generic;

namespace SME.SR.Infra.Dtos.Relatorios.CDEP
{
    public class FiltroRelatorioControleAcervoAutor
    {
        public string Usuario { get; set; }
        public string UsuarioRF { get; set; }
        public List<string> Autores { get; set; }
        public TipoAcervo TipoAcervo { get; set; }
        public long[] TiposAcervosPermitidos { get; set; }
    }
}
