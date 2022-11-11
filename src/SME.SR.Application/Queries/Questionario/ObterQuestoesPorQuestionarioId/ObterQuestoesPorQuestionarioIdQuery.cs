using System.Collections.Generic;
using MediatR;
using SME.SR.Infra;

namespace SME.SR.Application
{
    public class ObterQuestoesPorQuestionarioIdQuery : IRequest<IEnumerable<QuestaoDto>>
    {
        public ObterQuestoesPorQuestionarioIdQuery(long questionarioId, ObterRespostasFunc obterRespostas = null)
        {
            QuestionarioId = questionarioId;
            ObterRespostas = obterRespostas;            
        }

        public long QuestionarioId { get; }
        public ObterRespostasFunc ObterRespostas { get; }
    }
}