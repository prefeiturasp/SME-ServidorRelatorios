using MediatR;
using SME.SR.Data;

namespace SME.SR.Application
{
    public class ObterTurmaPorIdQuery : IRequest<Turma>
    {
        public ObterTurmaPorIdQuery(long id)
        {
            Id = id;
        }

        public long Id { get; }
    }
}
