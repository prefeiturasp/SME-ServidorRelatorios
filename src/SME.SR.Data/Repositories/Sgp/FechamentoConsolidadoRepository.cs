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

        public async Task<IEnumerable<FechamentoConsolidadoComponenteTurmaDto>> ObterFechamentoConsolidadoPorTurmas(string[] turmasCodigo)
        {
            var query = new StringBuilder(@" select f.id, f.dt_atualizacao DataAtualizacao, f.status, f.componente_curricular_id ComponenteCurricularCodigo,
                                                    f.professor_nome ProfessorNome, f.professor_rf ProfessorRf, t.turma_id TurmaCodigo, f.bimestre
                                                from consolidado_fechamento_componente_turma f
                                                inner join turma t on f.turma_id = t.id
                                               where not f.excluido 
                                                 and t.turma_id = ANY(@turmasCodigo) ");
            var parametros = new { turmasCodigo };

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas);
            return await conexao.QueryAsync<FechamentoConsolidadoComponenteTurmaDto>(query.ToString(), parametros);
        }
    }
}
