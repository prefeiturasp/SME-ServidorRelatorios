using MediatR;
using SME.SR.Infra;

namespace SME.SR.Application
{
    public class ObterTurmaResumoComDreUePorIdQuery : IRequest<TurmaResumoDto>
    {
        public ObterTurmaResumoComDreUePorIdQuery(long turmaId)
        {
            TurmaId = turmaId;
        }

        public long TurmaId { get; set; }
    }
}
