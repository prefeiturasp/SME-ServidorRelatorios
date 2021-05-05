using MediatR;
using SME.SR.Data;

namespace SME.SR.Application
{
    public class ObterComDreUePorTurmaIdQuery : IRequest<Turma>
    {
        public long TurmaId { get; set; }

        public ObterComDreUePorTurmaIdQuery(long turmaId)
        {
            TurmaId = turmaId;
        }
    }
}
