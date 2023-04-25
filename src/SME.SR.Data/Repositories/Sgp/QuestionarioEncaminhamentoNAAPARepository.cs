using Npgsql;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System.Threading.Tasks;

namespace SME.SR.Data.Repositories.Sgp
{
    public class QuestionarioEncaminhamentoNAAPARepository : QuestionarioRepository, IQuestionarioEncaminhamentoNAAPARepository
    {
        public QuestionarioEncaminhamentoNAAPARepository(VariaveisAmbiente variaveisAmbiente) : base(variaveisAmbiente)
        {
        }

        public async Task<long> ObterQuestionarioIdPorTipoESecao(int tipoQuestionario, string nomeComponenteSecao)
        {
            string query = @"select q.id
	                         from questionario q
	                         inner join secao_encaminhamento_naapa sen on sen.questionario_id = q.id and not sen.excluido
	                         where not q.excluido and q.tipo = @tipoQuestionario and sen.nome_componente = @nomeComponenteSecao ";

            await using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas);
            return await conexao.QueryFirstOrDefaultAsync<long>(query, new { tipoQuestionario, nomeComponenteSecao });
        }
    }
}
