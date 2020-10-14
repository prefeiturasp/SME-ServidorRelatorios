using Dapper;
using Npgsql;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class MathPoolCMRepository : IMathPoolCMRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public MathPoolCMRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<IEnumerable<MathPoolCM>> ObterPorFiltros(string codigoDre, string codigoUe, int anoTurma, int anoLetivo, int semestre)
        {
            StringBuilder query = new StringBuilder();

            query.Append("SELECT * from \"MathPoolCMs\" mpn where 1=1 ");

            if (!string.IsNullOrEmpty(codigoDre))
                query.Append("and \"DreEolCode\" = @codigoDre ");

            if (!string.IsNullOrEmpty(codigoUe))
                query.Append("and \"EscolaEolCode\" = @codigoUe ");

            if (anoTurma > 0)
                query.Append("and \"AnoTurma\" = @anoTurma ");

            if (anoLetivo > 0)
                query.Append("and \"AnoLetivo\" = @anoLetivo ");

            if (semestre > 0)
                query.Append("and \"Semestre\" = @semestre ");

            var parametros = new { codigoDre, codigoUe, anoTurma, anoLetivo, semestre };

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSondagem);
            return await conexao.QueryAsync<MathPoolCM>(query.ToString(), parametros);
        }
    }
}
