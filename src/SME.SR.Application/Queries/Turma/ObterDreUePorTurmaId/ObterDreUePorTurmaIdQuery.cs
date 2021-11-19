using MediatR;
using SME.SR.Data;

namespace SME.SR.Application
{
    public class ObterDreUePorTurmaIdQuery : IRequest<DreUe>
    {
        public ObterDreUePorTurmaIdQuery(long turmaId)
        {
            TurmaId = turmaId;
        }

        public long TurmaId { get; }
    }
}
