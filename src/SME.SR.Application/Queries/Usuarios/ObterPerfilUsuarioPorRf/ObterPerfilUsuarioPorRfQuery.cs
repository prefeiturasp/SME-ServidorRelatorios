using MediatR;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Application
{
    public class ObterPerfilUsuarioPorRfQuery : IRequest<IEnumerable<PerfilUsuarioDto>>
    {
        public ObterPerfilUsuarioPorRfQuery(IList<string> rfs)
        {
            Rfs = rfs;
        }

        public IList<string> Rfs { get; set; }
    }
}
