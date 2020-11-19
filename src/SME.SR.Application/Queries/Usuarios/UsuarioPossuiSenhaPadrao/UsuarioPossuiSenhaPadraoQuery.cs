using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Application
{
    public class UsuarioPossuiSenhaPadraoQuery : IRequest<bool>
    {
        public UsuarioPossuiSenhaPadraoQuery(string usuarioRf)
        {
            UsuarioRf = usuarioRf;
        }

        public string UsuarioRf { get; set; }
    }
}
