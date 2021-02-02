using MediatR;
using SME.SR.Data.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterNomeComponenteCurricularPorIdQueryHandler : IRequestHandler<ObterNomeComponenteCurricularPorIdQuery, string>
    {
        private readonly IComponenteCurricularRepository componenteCurricularRepository;

        public ObterNomeComponenteCurricularPorIdQueryHandler(IComponenteCurricularRepository componenteCurricularRepository)
        {
            this.componenteCurricularRepository = componenteCurricularRepository;
        }

        public async Task<string> Handle(ObterNomeComponenteCurricularPorIdQuery request, CancellationToken cancellationToken)
                => await componenteCurricularRepository.ObterNomeComponenteCurricularPorId(request.ComponenteCurricularId);
    }
}
