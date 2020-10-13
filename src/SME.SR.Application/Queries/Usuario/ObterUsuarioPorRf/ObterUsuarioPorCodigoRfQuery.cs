using MediatR;
using SME.SR.Data;

namespace SME.SR.Application
{
    public class ObterUsuarioPorCodigoRfQuery : IRequest<Usuario>
    {
        public string UsuarioRf { get; set; }
    }
}
