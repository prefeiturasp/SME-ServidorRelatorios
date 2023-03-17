using MediatR;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterQuestoesEncaminhamentoNAAPAPorIdENomeComponenteSecaoQueryHandler : IRequestHandler<ObterQuestoesEncaminhamentoNAAPAPorIdENomeComponenteSecaoQuery, IEnumerable<QuestaoDto>>
    {
        private readonly IMediator mediator;

        public ObterQuestoesEncaminhamentoNAAPAPorIdENomeComponenteSecaoQueryHandler(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<IEnumerable<QuestaoDto>> Handle(ObterQuestoesEncaminhamentoNAAPAPorIdENomeComponenteSecaoQuery request, CancellationToken cancellationToken)
        {
            var respostas = await mediator.Send(
                new ObterRespostasEncaminhamentoNAAPAPorIdENomeComponenteSecaoQuery(request.EncaminhamentoNaapaId, request.NomeComponenteSecao),
                cancellationToken);

            var questionarioId = await mediator.Send(
                new ObterQuestionarioIdEncaminhamentoNAAPAQuery(request.NomeComponenteSecao),
                cancellationToken);

            var questoes = await mediator.Send(
                new ObterQuestoesPorQuestionarioIdQuery(questionarioId, questaoId => respostas.Where(c => c.QuestaoId == questaoId)),
                cancellationToken);

            return questoes;
        }
    }
}
