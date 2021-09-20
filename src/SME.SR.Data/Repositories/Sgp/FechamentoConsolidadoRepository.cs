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

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp);
            return await conexao.QueryAsync<FechamentoConsolidadoComponenteTurmaDto>(query.ToString(), parametros);
        }

        public async Task<IEnumerable<FechamentoConsolidadoTurmaDto>> ObterFechamentoConsolidadoPorTurmasTodasUe(string[] turmasCodigo, int modalidade, int[] bimestres, int situacao)
        {
            var query = new StringBuilder(@"select
                                            u.ue_id as UeCodigo,
	                                        t.turma_id TurmaCodigo,
	                                        u.nome as NomeUe,
	                                        t.nome as NomeTurma,
	                                        t.modalidade_codigo as ModalidadeCodigo,
	                                        cfct.bimestre,
	                                        count(cfct.id) filter(where cfct.status in(0,1)) as NaoIniciado,
	                                        count(cfct.id) filter(where cfct.status = 2) as ProcessadoComPendencia,
	                                        count(cfct.id) filter(where cfct.status = 3) as ProcessadoComSucesso
                                       from consolidado_fechamento_componente_turma cfct 
	                                  inner join turma t on t.id = cfct.turma_id 
	                                  inner join ue u on u.id = t.ue_id 
                                      where t.ano_letivo = 2021
                                        and t.turma_id  = ANY(@turmasCodigo)
                                        and t.modalidade_codigo = @modalidade ");

            if(!bimestres.Any(b => b == -99))
                query.AppendLine(" and cfct.bimestre = ANY(@bimestres) ");

            if (situacao != -99)
                query.AppendLine(" and cfct.status = @situacao ");

            query.AppendLine(@" and not cfct.excluido
                                group by u.ue_id, t.turma_id, t.id, u.nome, t.nome, cfct.bimestre, t.modalidade_codigo
                                order by cfct.bimestre, t.nome; ");

            var parametros = new 
            {
                turmasCodigo, 
                modalidade,
                bimestres,
                situacao
            };

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp);
            return await conexao.QueryAsync<FechamentoConsolidadoTurmaDto>(query.ToString(), parametros);
        }
    }
}
