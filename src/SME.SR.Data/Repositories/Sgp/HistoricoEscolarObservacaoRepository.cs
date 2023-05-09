using Npgsql;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class HistoricoEscolarObservacaoRepository : IHistoricoEscolarObservacaoRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;
        public HistoricoEscolarObservacaoRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<IEnumerable<FiltroHistoricoEscolarAlunosDto>> ObterPorCodigosAlunosAsync(string[] codigosAlunos)
        {
            var query = @"select id, aluno_codigo as AlunoCodigo, observacao as ObservacaoComplementar
                        from historico_escolar_observacao 
                        where aluno_codigo = any(@codigosAlunos)";

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas);

            return await conexao.QueryAsync<FiltroHistoricoEscolarAlunosDto>(query, new { codigosAlunos });
        }
    }
}
