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

        public async Task<IEnumerable<FechamentoConsolidadoComponenteTurmaDto>> ObterFechamentoConsolidadoPorTurmas(string[] turmasCodigo, int[] semestres, int[] bimestres)
        {
            var query = new StringBuilder(@$" select * from (
                                            select f.id, f.dt_atualizacao DataAtualizacao, f.status, f.componente_curricular_id ComponenteCurricularCodigo,
                                                    f.professor_nome ProfessorNome, f.professor_rf ProfessorRf, t.turma_id TurmaCodigo, f.bimestre,
                                                    row_number() over (partition by f.componente_curricular_id, f.bimestre order by f.dt_atualizacao desc) sequencia
                                                from consolidado_fechamento_componente_turma f
                                                inner join turma t on f.turma_id = t.id
                                               where not f.excluido 
                                                 and t.turma_id = ANY(@turmasCodigo) 
                                                 {(semestres != null? " and t.semestre = ANY(@semestres)":"")}
                                                 {(bimestres != null? " and f.bimestre = ANY(@bimestres)":"")}
                                            ) x where x.sequencia = 1");
            var parametros = new { turmasCodigo, semestres, bimestres };

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas);
            return await conexao.QueryAsync<FechamentoConsolidadoComponenteTurmaDto>(query.ToString(), parametros);
        }

        public async Task<IEnumerable<FechamentoConsolidadoTurmaDto>> ObterFechamentoConsolidadoPorTurmasTodasUe(string dreCodigo, int modalidade, int[] bimestres, SituacaoFechamento? situacao, int anoLetivo, int semestre, bool exibirHistorico)
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
                                      inner join dre d on d.id = u.dre_id 
                                        where t.ano_letivo = @anoLetivo
                                        and d.dre_id  = @dreCodigo
                                        and t.modalidade_codigo = @modalidade ");

            if (bimestres != null)
                query.AppendLine(" and cfct.bimestre = ANY(@bimestres) ");

            if(semestre > 0)
                query.AppendLine(" and t.semestre = @semestre ");

            if (situacao != null)
                query.AppendLine(" and cfct.status = @situacao ");

            if(!exibirHistorico)
                query.AppendLine(" and not t.historica ");

            query.AppendLine(@" and not cfct.excluido
                                group by u.ue_id, t.turma_id, t.id, u.nome, t.nome, cfct.bimestre, t.modalidade_codigo
                                order by u.nome, t.nome, cfct.bimestre; ");

            var parametros = new
            {
                dreCodigo,
                modalidade,
                semestre,
                bimestres,
                situacao,
                anoLetivo
            };

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp);
            return await conexao.QueryAsync<FechamentoConsolidadoTurmaDto>(query.ToString(), parametros);
        }
    }
}
