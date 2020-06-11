using Dapper;
using Npgsql;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class PeriodoEscolarRepository : IPeriodoEscolarRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public PeriodoEscolarRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<IEnumerable<PeriodoEscolar>> ObterPeriodosEscolaresPorTipoCalendario(long tipoCalendarioId)
        {
            var query = PeriodoEscolarConsultas.ObterPorTipoCalendario;
            var parametros = new { TipoCalendarioId = tipoCalendarioId };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return await conexao.QueryAsync<PeriodoEscolar>(query, parametros);
            }
        }
    }
}
