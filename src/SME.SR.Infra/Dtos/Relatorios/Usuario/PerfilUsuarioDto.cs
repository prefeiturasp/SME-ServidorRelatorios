using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class PerfilUsuarioDto
    {
        public string Nome { get; set; }
        public List<UsuarioDto> Usuarios { get; set; }
    }
}
