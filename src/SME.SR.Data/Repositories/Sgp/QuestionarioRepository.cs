using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Npgsql;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;

namespace SME.SR.Data
{
    public class QuestionarioRepository : IQuestionarioRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public QuestionarioRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<long> ObterQuestionarioIdPorTipo(int tipoQuestionario)
        {
            const string query = @"select id 
                                    from questionario q
                                    where q.tipo = @tipoQuestionario";

            await using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas);
            return await conexao.QuerySingleOrDefaultAsync<long>(query, new { tipoQuestionario });            
        }

        public async Task<IEnumerable<Questao>> ObterQuestoesPorQuestionarioId(long questionarioId)
        {
            const string query = @"select q.id,
                                        q.ordem,
                                        q.nome,
                                        q.tipo, 
                                        op.id,
                                        op.questao_id as questaoid,
                                        op.ordem,
                                        op.nome,
                                        oqc.id,
                                        oqc.opcao_resposta_id as opcaorespostaid,
                                        oqc.questao_complementar_id as questaocomplementarid
                                    from questao q 
                                        left join opcao_resposta op on op.questao_id = q.id
                                        left join opcao_questao_complementar oqc on oqc.opcao_resposta_id = op.id
                                    where q.questionario_id = @questionarioId 
                                    order by q.id, op.id";            

            var lookup = new Dictionary<long, Questao>();
            
            await using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas);

            await conexao.QueryAsync<Questao, OpcaoResposta, OpcaoQuestaoComplementar, Questao>(query,
                (questao, opcaoResposta, opcaoQuestaoComplementar) =>
                {
                    if (!lookup.TryGetValue(questao.Id, out var q))
                    {
                        q = questao;
                        lookup.Add(q.Id, q);
                    }

                    var entidadeOpcaoResposta = q.OpcoesRespostas.FirstOrDefault(a => a.Id == opcaoResposta.Id);
                    
                    if (entidadeOpcaoResposta == null && opcaoResposta != null)
                    {
                        q.OpcoesRespostas.Add(opcaoResposta);
                        entidadeOpcaoResposta = opcaoResposta;
                    }

                    if (opcaoQuestaoComplementar != null && entidadeOpcaoResposta != null)
                        entidadeOpcaoResposta.QuestoesComplementares.Add(opcaoQuestaoComplementar);

                    return q;
                }, new { questionarioId });

            return lookup.Values;
        }
    }
}