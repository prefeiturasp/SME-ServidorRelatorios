using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class UsuarioLogadoDto
    {
        public string CodigoRf { get; set; }
        public string Login { get; set; }
        public string Nome { get; set; }
        public Guid PerfilAtual { get; set; }
    }
}
