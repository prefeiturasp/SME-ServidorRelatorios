using Dapper;
using Npgsql;
using SME.SR.Data.Queries;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Data.Repositories.Sgp
{
    public class PeriodoFechamentoRepository : IPeriodoFechamentoRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public PeriodoFechamentoRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<PeriodoFechamentoBimestre> ObterPeriodoFechamentoTurmaAsync(long ueId, long dreId, int anoLetivo, int bimestre, long? periodoEscolarId)
        {
            var query = PeriodoFechamentoConsultas.ObterPorTurma(bimestre, periodoEscolarId);
            var parametros = new
            {
                UeId = ueId,
                DreId = dreId,
                AnoLetivo = anoLetivo,
                Bimestre = bimestre,
                PeriodoEscolarId =  periodoEscolarId
            };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return await conexao.QueryFirstOrDefaultAsync<PeriodoFechamentoBimestre>(query, parametros);
            };
        }
    }
}
