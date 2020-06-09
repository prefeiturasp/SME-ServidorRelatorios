using Dapper;
using Npgsql;
using SME.SR.Workers.SGP.Commons;
using SME.SR.Workers.SGP.Commons.Config;
using SME.SR.Workers.SGP.Models;
using System.Threading.Tasks;

namespace SME.SR.Workers.SGP.Infra.Repositories
{
    public class ConselhoClasseAlunoRepository : IConselhoClasseAlunoRepository
    {
        public async Task<string> ObterParecerConclusivo(long conselhoClasseId, string codigoAluno)
        {
            var query = ConselhoClasseAlunoConsultas.ParecerConclusivo;
            var parametros = new { ConselhoClasseId = conselhoClasseId, CodigoAluno = codigoAluno };

            using (var conexao = new NpgsqlConnection(ConnectionStrings.ConexaoSgp))
            {
                return await conexao.QuerySingleOrDefaultAsync<string>(query, parametros);
            }
        }

        public async Task<RecomendacaoConselhoClasseAluno> ObterRecomendacoesPorFechamento(long fechamentoTurmaId, string codigoAluno)
        {
            var query = ConselhoClasseAlunoConsultas.Recomendacoes;
            var parametros = new { FechamentoTurmaId = fechamentoTurmaId, CodigoAluno = codigoAluno };

            using (var conexao = new NpgsqlConnection(ConnectionStrings.ConexaoSgp))
            {
                return await conexao.QueryFirstOrDefaultAsync<RecomendacaoConselhoClasseAluno>(query, parametros);
            }
        }
    }
}
