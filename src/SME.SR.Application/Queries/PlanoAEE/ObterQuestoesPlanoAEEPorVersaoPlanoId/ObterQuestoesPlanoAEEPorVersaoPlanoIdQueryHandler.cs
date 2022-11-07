using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SME.SR.Infra;

namespace SME.SR.Application
{
    public class ObterQuestoesPlanoAEEPorVersaoPlanoIdQueryHandler : IRequestHandler<ObterQuestoesPlanoAEEPorVersaoPlanoIdQuery, IEnumerable<QuestaoDto>>
    {
        private readonly IMediator mediator;

        public ObterQuestoesPlanoAEEPorVersaoPlanoIdQueryHandler(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<IEnumerable<QuestaoDto>> Handle(ObterQuestoesPlanoAEEPorVersaoPlanoIdQuery request, CancellationToken cancellationToken)
        {
            var respostasPlano = await mediator.Send(new ObterRespostasPlanoAEEPorVersaoPlanoIdQuery(request.VersaoPlanoId),
                cancellationToken);

            var questionarioId = await mediator.Send(new ObterQuestionarioIdPlanoAEEQuery(), cancellationToken);

            return await mediator.Send(
                new ObterQuestoesPorQuestionarioIdQuery(questionarioId,
                    questaoId => respostasPlano.Where(c => c.QuestaoId == questaoId)), cancellationToken);
        }
    }
}