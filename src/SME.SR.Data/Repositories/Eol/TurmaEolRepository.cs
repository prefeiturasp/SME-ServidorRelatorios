using Dapper;
using SME.SR.Infra;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class TurmaEolRepository : ITurmaEolRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public TurmaEolRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }
        public async Task<Turma> ObterTurmaSondagemPorCodigo(long turmaCodigo)
        {
            var query = @"select cd_turma_escola as codigo, an_letivo as anoLetivo, dc_turma_escola as nome, 5 as ModalidadeCodigo from turma_escola where cd_turma_escola = @turmaCodigo";

            using var conexao = new SqlConnection(variaveisAmbiente.ConnectionStringEol);

            return await conexao.QueryFirstOrDefaultAsync<Turma>(query, new { turmaCodigo });

        }
    }
}
