using System;

namespace SME.SR.Infra
{
    public class UsuarioDto
    {
        public string Login { get; set; }
        public string Nome { get; set; }
        public string Situacao { get; set; }
        public string UltimoAcesso { get; set; }
    }
}
