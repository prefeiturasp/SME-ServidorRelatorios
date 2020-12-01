using MediatR;

namespace SME.SR.Application
{
    public class ObterNomeComponenteCurricularPorIdQuery : IRequest<string>
    {
        public ObterNomeComponenteCurricularPorIdQuery(long componenteCurricularId)
        {
            ComponenteCurricularId = componenteCurricularId;
        }

        public long ComponenteCurricularId { get; set; }
    }
}
