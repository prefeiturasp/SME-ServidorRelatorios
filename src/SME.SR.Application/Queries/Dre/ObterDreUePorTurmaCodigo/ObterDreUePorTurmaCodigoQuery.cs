using MediatR;
using SME.SR.Data;

namespace SME.SR.Application
{
    public class ObterDreUePorTurmaCodigoQuery : IRequest<DreUe>
    {
        public ObterDreUePorTurmaCodigoQuery(string turmaCodigo)
        {
            TurmaCodigo = turmaCodigo;
        }

        public string TurmaCodigo { get; }
    }
}
