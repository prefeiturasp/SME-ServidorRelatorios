using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterUsuariosAePorCpfsQuery : IRequest<IEnumerable<UsuarioAEDto>>
    {
        public ObterUsuariosAePorCpfsQuery(string[] cpfs)
        {
            Cpfs = cpfs;
        }

        public string[] Cpfs { get; set; }
    }
}
