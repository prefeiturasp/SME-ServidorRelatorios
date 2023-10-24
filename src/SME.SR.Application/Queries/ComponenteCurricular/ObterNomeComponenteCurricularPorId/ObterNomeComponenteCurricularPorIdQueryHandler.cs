using MediatR;
using SME.SR.Data.Interfaces;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterNomeComponenteCurricularPorIdQueryHandler : IRequestHandler<ObterNomeComponenteCurricularPorIdQuery, string>
    {
        private readonly IComponenteCurricularRepository componenteCurricularRepository;
        private readonly IMediator mediator;

        public ObterNomeComponenteCurricularPorIdQueryHandler(IComponenteCurricularRepository componenteCurricularRepository, IMediator mediator)
        {
            this.componenteCurricularRepository = componenteCurricularRepository;
            this.mediator = mediator;
        }

        public async Task<string> Handle(ObterNomeComponenteCurricularPorIdQuery request, CancellationToken cancellationToken)
        {
            var componenteCurricular = await mediator.Send(new ObterComponentesCurricularesEolPorIdsQuery(request.ComponenteCurricularId));
            if (componenteCurricular.Any())
                return componenteCurricular.FirstOrDefault().Disciplina;
            return string.Empty;           
        }
    }
}
