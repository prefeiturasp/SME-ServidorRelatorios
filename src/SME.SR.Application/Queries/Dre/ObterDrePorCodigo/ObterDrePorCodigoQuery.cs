using MediatR;
using SME.SR.Data;

namespace SME.SR.Application
{
    public class ObterDrePorCodigoQuery : IRequest<Dre>
    {
        public ObterDrePorCodigoQuery() { }

        public ObterDrePorCodigoQuery(string dreCodigo)
        {
            DreCodigo = dreCodigo;
        }

        public string DreCodigo { get; set; }
    }
}
