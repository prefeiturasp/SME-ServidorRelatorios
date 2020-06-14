using Dapper;
using Npgsql;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class AulaRepository : IAulaRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public AulaRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<int> ObterAulasDadas(string codigoTurma, string disciplinaId, long tipoCalendarioId, int bimestre)
        {
            var query = AulaConsultas.AulasCumpridas;
            var parametros = new { 
                CodigoTurma = codigoTurma, 
                DisciplinaId = disciplinaId,
                TipoCalendarioId = tipoCalendarioId,
                Bimestre = bimestre
            };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return await conexao.QuerySingleOrDefaultAsync<int>(query, parametros);
            }
        }
    }
}
