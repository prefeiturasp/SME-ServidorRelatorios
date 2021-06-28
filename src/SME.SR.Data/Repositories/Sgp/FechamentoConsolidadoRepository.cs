using Dapper;
using Npgsql;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Data.Repositories.Sgp
{
    public class FechamentoConsolidadoRepository : IFechamentoConsolidadoRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public FechamentoConsolidadoRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<IEnumerable<FechamentoConsolidadoComponenteTurmaDto>> ObterFechamentoConsolidadoPorTurmasBimestre(long[] turmasId, int bimestre, int situacaoFechamento)
        {
            var query = new StringBuilder(@" select id, dt_atualizacao DataAtualizacao, status, componente_curricular_id ComponenteCurricularCodigo,
                                                    professor_nome ProfessorNome, professor_rf ProfessorRf, turma_id TurmaId, bimestre
                                                from consolidado_fechamento_componente_turma 
                                               where not excluido 
                                                 and turma_id = ANY(@turmasId) ");

            if (bimestre != -99)
                query.AppendLine(@"and bimestre = @bimestre");

            if (situacaoFechamento != -99)
                query.AppendLine(@"and EXISTS(select 1 from consolidado_fechamento_componente_turma 
                                              where turma_id = @turmaId and bimestre = @bimestre and status = @situacaoFechamento)");

            var parametros = new { turmasId, bimestre, situacaoFechamento };

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp);
            return await conexao.QueryAsync<FechamentoConsolidadoComponenteTurmaDto>(query.ToString(), parametros);
        }
    }
}
