using MediatR;
using SME.SR.Data;

namespace SME.SR.Application
{
    public class ObterDreUePorTurmaIdQuery : IRequest<DreUe>
    {
        public ObterDreUePorTurmaIdQuery(string turmaCodigo)
        {
            TurmaCodigo = turmaCodigo;
        }

        public string TurmaCodigo { get; }
    }
}
