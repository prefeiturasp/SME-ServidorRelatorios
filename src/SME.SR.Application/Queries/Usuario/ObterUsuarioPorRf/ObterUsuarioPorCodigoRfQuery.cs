using MediatR;
using SME.SR.Data;

namespace SME.SR.Application
{
    public class ObterUsuarioPorCodigoRfQuery : IRequest<Usuario>
    {
        public ObterUsuarioPorCodigoRfQuery()
        {

        }
        public ObterUsuarioPorCodigoRfQuery(string usuarioRf)
        {
            UsuarioRf = usuarioRf;
        }
        public string UsuarioRf { get; set; }
    }
}
