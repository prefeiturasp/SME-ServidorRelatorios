using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class PerfilUsuarioDto
    {
        public string Nome { get; set; }
        public IEnumerable<UsuarioDto> Usuarios { get; set; }
    }
}
