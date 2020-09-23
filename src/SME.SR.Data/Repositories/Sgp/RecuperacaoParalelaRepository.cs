using Dapper;
using Npgsql;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class RecuperacaoParalelaRepository : IRecuperacaoParalelaRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public RecuperacaoParalelaRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<IEnumerable<ResumoPAPTotalAlunosAnoDto>> ListarTotalAlunosSeries(int? periodo, string dreId, string ueId, int? cicloId, string turmaId, string ano, int anoLetivo)
        {
            var query = new StringBuilder();
            query.Append(@" select
                                count ( distinct aluno_id) as total,
                                turma.ano,
                                tipo_ciclo.descricao as Ciclo
                            from recuperacao_paralela rp
                                inner join turma on rp.turma_id = turma.id
                                inner join tipo_ciclo_ano tca on turma.modalidade_codigo = tca.modalidade and turma.ano = tca.ano
                                inner join tipo_ciclo on tca.tipo_ciclo_id = tipo_ciclo.id
                                inner join recuperacao_paralela_periodo_objetivo_resposta rpp on rp.id = rpp.recuperacao_paralela_id
                                inner join ue on ue.id = turma.ue_id
                                inner join dre on dre.id = ue.dre_id
                                where rp.excluido = false 
                                and rp.ano_letivo = @anoLetivo ");

            if (!string.IsNullOrEmpty(dreId) && dreId != "0")
                query.AppendLine(" and dre.dre_id = @dreId");

            if (!string.IsNullOrEmpty(ueId) && ueId != "0")
                query.AppendLine(" and ue.ue_id = @ueId");

            if (cicloId != null && cicloId != 0)
                query.AppendLine(" and tca.tipo_ciclo_id = @cicloId");

            if (!string.IsNullOrEmpty(ano) && ano != "0")
                query.AppendLine("and turma.ano = @ano");

            if (periodo != null && periodo != 0)
                query.AppendLine(" and rpp.periodo_recuperacao_paralela_id = @periodoId");

            if (!string.IsNullOrEmpty(turmaId) && turmaId != "0")
                query.AppendLine(" and turma.turma_id = @turmaId");

            query.Append(@" group by
                                turma.ano,
                                tipo_ciclo.descricao");

            var parametros = new { dreId, ueId, cicloId, turmaId, ano, periodo, anoLetivo };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return await conexao.QueryAsync<ResumoPAPTotalAlunosAnoDto>(query.ToString(), parametros);
            }
        }

        public async Task<RecuperacaoParalelaPeriodo> ObterPeriodoPorId(long id)
        {
            var query = @"select id, nome, descricao, bimestre_edicao
                         where not excluido and id = @Id";
            var parametros = new { Id = id } ;

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return await conexao.QueryFirstOrDefaultAsync(query, parametros);
            }
        }
    }
}
