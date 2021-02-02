using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Application
{
    public class ObterDadosUsuarioCoreSSOPorRfQuery : IRequest<UsuarioCoreSSO>
    {
        public ObterDadosUsuarioCoreSSOPorRfQuery(string usuarioRf)
        {
            UsuarioRf = usuarioRf;
        }

        public string UsuarioRf { get; set; }
    }
}
