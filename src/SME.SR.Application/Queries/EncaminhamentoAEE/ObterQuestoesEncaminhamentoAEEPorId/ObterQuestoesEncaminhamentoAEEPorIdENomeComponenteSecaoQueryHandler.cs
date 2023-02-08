using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SME.SR.Data.Models;
using SME.SR.Infra;

namespace SME.SR.Application
{
    public class ObterQuestoesEncaminhamentoAEEPorIdENomeComponenteSecaoQueryHandler : IRequestHandler<ObterQuestoesEncaminhamentoAEEPorIdENomeComponenteSecaoQuery, IEnumerable<QuestaoDto>>
    {
        private readonly IMediator mediator;

        public ObterQuestoesEncaminhamentoAEEPorIdENomeComponenteSecaoQueryHandler(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<IEnumerable<QuestaoDto>> Handle(ObterQuestoesEncaminhamentoAEEPorIdENomeComponenteSecaoQuery request, CancellationToken cancellationToken)
        {
            var respostasEncaminhamento = await mediator.Send(new ObterRespostasEncaminhamentoAEEPorIdENomeComponenteSecaoQuery(request.EncaminhamentoAeeId, request.NomeComponenteSecao),
                cancellationToken);

            var questionarioId = await mediator.Send(new ObterQuestionarioIdEncaminhamentoAEEQuery(request.NomeComponenteSecao), cancellationToken);

            return (await mediator.Send(
                new ObterQuestoesPorQuestionarioIdQuery(questionarioId,
                    questaoId => respostasEncaminhamento.Where(c => c.QuestaoId == questaoId)), cancellationToken));

        }
    }
}