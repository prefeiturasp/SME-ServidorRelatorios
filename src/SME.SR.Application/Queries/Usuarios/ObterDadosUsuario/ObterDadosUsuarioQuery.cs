using MediatR;
using SME.SR.Data;

namespace SME.SR.Application
{
    public class ObterDadosUsuarioQuery : IRequest<Usuario>
    {
        public string CodigoRf { get; set; }
    }
}
