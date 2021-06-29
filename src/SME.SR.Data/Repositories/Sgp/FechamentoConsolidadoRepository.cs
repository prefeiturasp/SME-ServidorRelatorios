using Dapper;
using Npgsql;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<IEnumerable<FechamentoConsolidadoComponenteTurmaDto>> ObterFechamentoConsolidadoPorTurmasBimestre(string[] turmasCodigo, int[] bimestres, int? situacaoFechamento)
        {
            var query = new StringBuilder(@" select f.id, f.dt_atualizacao DataAtualizacao, f.status, f.componente_curricular_id ComponenteCurricularCodigo,
                                                    f.professor_nome ProfessorNome, f.professor_rf f.ProfessorRf, f.turma_id TurmaId, f.bimestre
                                                from consolidado_fechamento_componente_turma f
                                                inner join turma t on f.turma_id = t.id
                                               where not f.excluido 
                                                 and t.turma_id = ANY(@turmasCodigo) ");

            if (bimestres != null && bimestres.Any())
                query.AppendLine(@"and f.bimestre = ANY(@bimestres)");

            if (situacaoFechamento.HasValue)
                query.AppendLine(@"and EXISTS(select 1 from consolidado_fechamento_componente_turma f2
                                              inner join turma t2 on f2.turma_id = t2.id
                                              where t2.turma_id = ANY(@turmasCodigo) and f2.bimestre = ANY(@bimestres) 
                                                and f2.status = @situacaoFechamento)");

            var parametros = new { turmasCodigo, bimestres, situacaoFechamento };

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp);
            return await conexao.QueryAsync<FechamentoConsolidadoComponenteTurmaDto>(query.ToString(), parametros);
        }
    }
}
