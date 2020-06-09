using Dapper;
using Npgsql;
using SME.SR.Workers.SGP.Commons;
using SME.SR.Workers.SGP.Commons.Config;
using SME.SR.Workers.SGP.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Workers.SGP.Infra
{
    public class PeriodoEscolarRepository : IPeriodoEscolarRepository
    {
        public async Task<IEnumerable<PeriodoEscolar>> ObterPeriodosEscolaresPorTipoCalendario(long tipoCalendarioId)
        {
            var query = PeriodoEscolarConsultas.ObterPorTipoCalendario;
            var parametros = new { TipoCalendarioId = tipoCalendarioId };

            using (var conexao = new NpgsqlConnection(ConnectionStrings.ConexaoSgp))
            {
                return await conexao.QueryAsync<PeriodoEscolar>(query, parametros);
            }
        }
    }
}
