using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;

namespace SME.SR.Application
{
    public delegate IEnumerable<RespostaQuestaoDto> ObterRespostasFunc(long questaoId);
    
    public class ObterQuestoesPorQuestionarioIdQueryHandler : IRequestHandler<ObterQuestoesPorQuestionarioIdQuery, IEnumerable<QuestaoDto>>
    {
        private readonly IQuestionarioRepository questionarioRepository;

        public ObterQuestoesPorQuestionarioIdQueryHandler(IQuestionarioRepository questionarioRepository)
        {
            this.questionarioRepository = questionarioRepository ?? throw new ArgumentNullException(nameof(questionarioRepository));
        }

        public async Task<IEnumerable<QuestaoDto>> Handle(ObterQuestoesPorQuestionarioIdQuery request, CancellationToken cancellationToken)
        {
            var questoes = (await questionarioRepository.ObterQuestoesPorQuestionarioId(request.QuestionarioId)).ToList();

            var questoesComplementares = questoes
                .Where(q => q.OpcoesRespostas.Any(a => a.QuestoesComplementares.Any()))
                .SelectMany(q => q.OpcoesRespostas.Where(c => c.QuestoesComplementares.Any()).SelectMany(a => a.QuestoesComplementares.Select(qc => qc.QuestaoComplementarId)))
                .Distinct();

            return questoes
                .Where(q => !questoesComplementares.Contains(q.Id))
                .Select(q => ObterQuestao(q.Id, questoes, request.ObterRespostas))
                .OrderBy(q => q.Ordem)
                .ToArray();
        }
        
        private IEnumerable<QuestaoDto> ObterQuestoes(IEnumerable<OpcaoQuestaoComplementar> questoesComplementares, IReadOnlyCollection<Questao> questoes, ObterRespostasFunc obterRespostas)
        {
            return questoesComplementares.Select(questaoComplementar => ObterQuestao(questaoComplementar.QuestaoComplementarId, questoes, obterRespostas));
        }

        private QuestaoDto ObterQuestao(long questaoId, IReadOnlyCollection<Questao> questoes, ObterRespostasFunc obterRespostas)
        {
            var questao = questoes.FirstOrDefault(c => c.Id == questaoId);

            if (questao == null)
                return new QuestaoDto();

            return new QuestaoDto
            {
                Id = questao.Id,
                Ordem = questao.Ordem,
                Nome = questao.Nome,
                NomeComponente = questao.NomeComponente,
                Tipo = questao.Tipo,
                OpcaoResposta = questao.OpcoesRespostas.Select(opcaoResposta =>
                    {
                        return new OpcaoRespostaDto
                        {
                            Id = opcaoResposta.Id,
                            QuestaoId = opcaoResposta.QuestaoId,
                            Nome = opcaoResposta.Nome,
                            Ordem = opcaoResposta.Ordem,
                            QuestoesComplementares = opcaoResposta.QuestoesComplementares != null ?
                                ObterQuestoes(opcaoResposta.QuestoesComplementares, questoes, obterRespostas)
                                    .OrderBy(a => a.Ordem)
                                    .ToList() :
                                null
                        };
                    })
                    .OrderBy(a => a.Ordem).ToArray(),
                Respostas = obterRespostas?.Invoke(questaoId)
            };
        }        
    }
}