using MediatR;
using SME.SR.Data;

namespace SME.SR.Application
{
    public class ObterDreUePorDreCodigoQuery : IRequest<DreUe>
    {
        public ObterDreUePorDreCodigoQuery(string dreCodigo,string ueCodigo)
        {
            DreCodigo = dreCodigo;
            UeCodigo = ueCodigo;
        }

        public string DreCodigo { get; }
        public string UeCodigo { get; }
    }
}
