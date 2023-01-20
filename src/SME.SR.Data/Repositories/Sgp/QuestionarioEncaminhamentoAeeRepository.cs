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
    public class QuestionarioEncaminhamentoAeeRepository : QuestionarioRepository, IQuestionarioEncaminhamentoAeeRepository
    {
        public QuestionarioEncaminhamentoAeeRepository(VariaveisAmbiente variaveisAmbiente):base(variaveisAmbiente)
        {}

        public async Task<long> ObterQuestionarioIdPorTipoESecao(int tipoQuestionario, string nomeComponenteSecao)
        {
            const string query = @"select q.id 
                                    from questionario q
                                    inner join secao_encaminhamento_aee sea on sea.questionario_id = q.id
                                    where q.tipo = @tipoQuestionario and sea.nome_componente = @nomeComponenteSecao ";

            await using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas);
            return (await conexao.QueryFirstOrDefaultAsync<long>(query, new { tipoQuestionario, nomeComponenteSecao }));
        }
    }
}