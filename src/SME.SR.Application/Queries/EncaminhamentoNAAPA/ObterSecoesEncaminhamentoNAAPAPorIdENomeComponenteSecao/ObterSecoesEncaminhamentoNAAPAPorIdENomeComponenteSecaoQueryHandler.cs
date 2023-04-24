using MediatR;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterSecoesEncaminhamentoNAAPAPorIdENomeComponenteSecaoQueryHandler : IRequestHandler<ObterSecoesEncaminhamentoNAAPAPorIdENomeComponenteSecaoQuery, IEnumerable<SecaoEncaminhamentoNAAPADto>>
    {
        private readonly IMediator mediator;

        public ObterSecoesEncaminhamentoNAAPAPorIdENomeComponenteSecaoQueryHandler(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<IEnumerable<SecaoEncaminhamentoNAAPADto>> Handle(ObterSecoesEncaminhamentoNAAPAPorIdENomeComponenteSecaoQuery request, CancellationToken cancellationToken)
        {
            var secoes = await mediator.Send(new ObterSecoesItineranciaEncaminhamentoNAAPAPorIdQuery(request.EncaminhamentoNaapaId, request.NomeComponenteSecao));

            if (secoes == null || !secoes.Any())
                return default;

            var respostas = await mediator.Send(
                new ObterRespostasEncaminhamentoNAAPAPorIdENomeComponenteSecaoQuery(request.EncaminhamentoNaapaId, request.NomeComponenteSecao),
                cancellationToken);

            var questionarioId = await mediator.Send(
                new ObterQuestionarioIdEncaminhamentoNAAPAQuery(request.NomeComponenteSecao),
                cancellationToken);

            foreach (var secao in secoes)
            {
                var respostasSecao = respostas.Where(t => t.SecaoId == secao.SecaoId);

                secao.Questoes = await mediator.Send(
                    new ObterQuestoesPorQuestionarioIdQuery(questionarioId, questaoId => respostasSecao.Where(c => c.QuestaoId == questaoId)),
                    cancellationToken);
            }

            return secoes;
        }
    }
}
