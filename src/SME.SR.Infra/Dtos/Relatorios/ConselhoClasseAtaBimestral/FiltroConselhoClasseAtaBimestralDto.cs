using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class FiltroConselhoClasseAtaBimestralDto
    {
        public int AnoLetivo { get; set; }
        public string DreCodigo { get; set; }
        public string UeCodigo { get; set; }
        public string UsuarioRF { get; set; }
        public string UsuarioNome { get; set; }
        public IEnumerable<string> TurmasCodigo { get; set; }
        public int Bimestre { get; set; }
    }
}
