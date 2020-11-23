using MediatR;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Application
{
    public class ObterSituacaoUsuarioPorRfQuery : IRequest<SituacaoUsuario>
    {
        public ObterSituacaoUsuarioPorRfQuery(string usuarioRf)
        {
            UsuarioRf = usuarioRf;
        }

        public string UsuarioRf { get; set; }
    }
}
