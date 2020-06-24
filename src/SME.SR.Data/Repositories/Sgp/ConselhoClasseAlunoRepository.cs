using Dapper;
using Npgsql;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class ConselhoClasseAlunoRepository : IConselhoClasseAlunoRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public ConselhoClasseAlunoRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<string> ObterParecerConclusivo(long conselhoClasseId, string codigoAluno)
        {
            var query = ConselhoClasseAlunoConsultas.ParecerConclusivo;
            var parametros = new { ConselhoClasseId = conselhoClasseId, CodigoAluno = codigoAluno };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return await conexao.QuerySingleOrDefaultAsync<string>(query, parametros);
            }
        }

        public async Task<RecomendacaoConselhoClasseAluno> ObterRecomendacoesPorFechamento(long fechamentoTurmaId, string codigoAluno)
        {
            var query = ConselhoClasseAlunoConsultas.Recomendacoes;
            var parametros = new { FechamentoTurmaId = fechamentoTurmaId, CodigoAluno = codigoAluno };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return await conexao.QueryFirstOrDefaultAsync<RecomendacaoConselhoClasseAluno>(query, parametros);
            }
        }

        public async Task<bool> PossuiConselhoClasseCadastrado(long conselhoClasseId, string codigoAluno)
        {
            var query = ConselhoClasseAlunoConsultas.ObterPorConselhoClasseId;
            var parametros = new { ConselhoClasseId = conselhoClasseId, CodigoAluno = codigoAluno };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return await conexao.QuerySingleOrDefaultAsync<bool>(query, parametros);
            }
        }
    }
}
