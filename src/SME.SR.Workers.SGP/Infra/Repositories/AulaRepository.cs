using Dapper;
using Npgsql;
using SME.SR.Workers.SGP.Commons;
using SME.SR.Workers.SGP.Commons.Config;
using System.Threading.Tasks;

namespace SME.SR.Workers.SGP.Infra
{
    public class AulaRepository : IAulaRepository
    {
        public async Task<int> ObterAulasDadas(string codigoTurma, string disciplinaId, long tipoCalendarioId, int bimestre)
        {
            var query = AulaConsultas.AulasCumpridas;
            var parametros = new { 
                CodigoTurma = codigoTurma, 
                DisciplinaId = disciplinaId,
                TipoCalendarioId = tipoCalendarioId,
                Bimestre = bimestre
            };

            using (var conexao = new NpgsqlConnection(ConnectionStrings.ConexaoSgp))
            {
                return await conexao.QuerySingleOrDefaultAsync<int>(query, parametros);
            }
        }
    }
}
