using Dapper;
using Npgsql;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class AulaPrevistaBimestreRepository : IAulaPrevistaBimestreRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public AulaPrevistaBimestreRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        const string Select = @" select t.nome as TurmaNome, coalesce(cc.descricao_sgp, cc.id as ComponenteCurricularId, cc.descricao) as ComponenteCurricularNome,
                                     p.bimestre, p.periodo_inicio as DataInicio, p.periodo_fim as DataFim, apb.aulas_previstas as Previstas,
                                     SUM(a.quantidade) filter (where a.tipo_aula = 1 and a.aula_cj = false) as CriadasTitular,
                                     SUM(a.quantidade) filter (where a.tipo_aula = 1 and a.aula_cj = true) as CriadasCJ,
                                     SUM(a.quantidade) filter (where a.tipo_aula = 1 and rf.id is not null and a.aula_cj = false) as CumpridasTitular,
                                     SUM(a.quantidade) filter (where a.tipo_aula = 1 and rf.id is not null and a.aula_cj = true) as CumpridasCj,
                                     SUM(a.quantidade) filter (where a.tipo_aula = 2 and rf.id is not null ) as Reposicoes
                         from periodo_escolar p
                         inner join tipo_calendario tp on p.tipo_calendario_id = tp.id
                         left join aula_prevista ap on ap.tipo_calendario_id = p.tipo_calendario_id
                         left join aula_prevista_bimestre apb on ap.id = apb.aula_prevista_id and p.bimestre = apb.bimestre
                         left join turma t on t.turma_id = ap.turma_id
                         left join componente_curricular cc on cc.id = ap.disciplina_id::bigint
                         left join aula a on a.turma_id = ap.turma_id and
                         				a.disciplina_id = ap.disciplina_id and
			                            a.tipo_calendario_id = p.tipo_calendario_id and
				                        a.data_aula BETWEEN p.periodo_inicio AND p.periodo_fim
                                        and (a.id is null or not a.excluido)
                         left join registro_frequencia rf on a.id = rf.aula_id ";

        const string GroupOrderBy = @" group by t.nome, cc.id, cc.descricao_sgp, cc.descricao, p.bimestre, p.periodo_inicio, p.periodo_fim, apb.aulas_previstas, apb.Id, p.periodo_inicio, p.periodo_fim ; ";



        public async Task<IEnumerable<AulaPrevistaBimestreQuantidade>> ObterBimestresAulasPrevistasPorFiltro(long turmaId, long componenteCurricularId)
        {
            StringBuilder query = new StringBuilder();

            query.Append(Select);
            query.Append(@" where tp.situacao and not tp.excluido and
                        t.id = @turmaId and
                        cc.id = @componenteCurricularId ");
            query.Append(GroupOrderBy);

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return await conexao.QueryAsync<AulaPrevistaBimestreQuantidade>(query.ToString(), new { turmaId, componenteCurricularId });
            }            
        }

    }
}
