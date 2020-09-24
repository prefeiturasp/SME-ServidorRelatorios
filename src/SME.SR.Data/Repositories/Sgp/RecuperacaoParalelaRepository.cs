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

        public async Task<IEnumerable<RetornoResumoPAPTotalAlunosAnoDto>> ListarTotalAlunosSeries(int? periodoId, string dreId, string ueId, int? cicloId, string turmaId, string ano, int anoLetivo)
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
                                inner join dre on dre.id = ue.dre_id");

            MontarWhereRecuperacaoParalela(query, dreId, ueId, cicloId, ano, periodoId, turmaId, null);
            query.Append(@" group by
                                turma.ano,
                                tipo_ciclo.descricao");

            var parametros = new { dreId, ueId, cicloId, turmaId, ano, periodoId, anoLetivo };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return await conexao.QueryAsync<RetornoResumoPAPTotalAlunosAnoDto>(query.ToString(), parametros);
            }
        }

        public async Task<IEnumerable<RetornoResumoPAPTotalAlunosAnoFrequenciaDto>> ListarTotalEstudantesPorFrequencia(int? periodoId, string dreId, string ueId, int? cicloId, string turmaId, string ano, int anoLetivo)
        {
            var query = new StringBuilder();
            query.Append(@"select
	                            count(distinct aluno_id) as total,
	                            turma.ano,
	                            tipo_ciclo.descricao as Ciclo,
                                tipo_ciclo.id as CicloId,
                                rpr.id as RespostaId,
	                            rpr.nome as frequencia
                            from recuperacao_paralela rp
	                            inner join turma on rp.turma_id = turma.id
	                            inner join tipo_ciclo_ano tca on turma.modalidade_codigo = tca.modalidade and turma.ano = tca.ano
	                            inner join tipo_ciclo on tca.tipo_ciclo_id = tipo_ciclo.id
	                            inner join recuperacao_paralela_periodo_objetivo_resposta rpp on rp.id = rpp.recuperacao_paralela_id
	                            inner join recuperacao_paralela_resposta rpr on rpp.resposta_id = rpr.id
                                inner join  ue on ue.id = turma.ue_id
                                inner join dre on dre.id = ue.dre_id");
            MontarWhereRecuperacaoParalela(query, dreId, ueId, cicloId, ano, periodoId, turmaId, null);
            query.AppendLine("and rpp.objetivo_id = 4");
            query.Append(@"group by
	                            turma.nome,
	                            turma.ano,
	                            tipo_ciclo.descricao,
	                            rpr.nome,
                                rpr.id,
                                tipo_ciclo.id
                            order by rpr.ordem");

            var parametros = new { dreId, ueId, cicloId, turmaId, ano, periodoId, anoLetivo };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return await conexao.QueryAsync<RetornoResumoPAPTotalAlunosAnoFrequenciaDto>(query.ToString(), parametros);
            }
        }

        public async Task<IEnumerable<RetornoResumoPAPTotalResultadoDto>> ListarTotalResultado(int? periodoId, string dreId, string ueId, int? cicloId, string turmaId, string ano, int anoLetivo, int? pagina)
        {
            if (pagina == 0) pagina = 1;
            StringBuilder query = new StringBuilder();
            query.AppendLine("select");
            MontarCamposResumo(query);
            MontarFromResumo(query);
            MontarWhereRecuperacaoParalela(query, dreId, ueId, cicloId, ano, periodoId, turmaId, null);
            query.AppendLine("and e.id NOT IN (1,2)");
            query.AppendLine("and e.excluido = false");
            query.AppendLine("and o.excluido = false");
            query.AppendLine("and rpr.excluido = false");
            query.AppendLine("group by");
            query.AppendLine("turma.nome,");
            query.AppendLine("turma.ano,");
            query.AppendLine("tipo_ciclo.id,");
            query.AppendLine("tipo_ciclo.descricao,");
            query.AppendLine("rpr.nome,");
            query.AppendLine("o.nome,");
            query.AppendLine("e.descricao,");
            query.AppendLine("o.ordem,");
            query.AppendLine("tipo_ciclo.descricao,");
            query.AppendLine("e.id,");
            query.AppendLine("o.id,");
            query.AppendLine("rpr.id");
            query.AppendLine("order by");
            query.AppendLine("o.ordem,");
            query.AppendLine("rpr.ordem;");

            var parametros = new { dreId, ueId, cicloId, turmaId, ano, periodoId, pagina, anoLetivo };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return await conexao.QueryAsync<RetornoResumoPAPTotalResultadoDto>(query.ToString(), parametros);
            }
        }

        public async Task<IEnumerable<RetornoResumoPAPTotalResultadoDto>> ListarTotalResultadoEncaminhamento(int? periodoId, string dreId, string ueId, int? cicloId, string turmaId, string ano, int anoLetivo, int? pagina)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("select");
            MontarCamposResumo(query);
            MontarFromResumo(query);
            MontarWhereRecuperacaoParalela(query, dreId, ueId, cicloId, ano, periodoId, turmaId, pagina);
            query.AppendLine("and e.id = 1");
            query.AppendLine("and e.excluido = false");
            query.AppendLine("and o.excluido = false");
            query.AppendLine("and rpr.excluido = false");
            query.AppendLine("group by");
            query.AppendLine("turma.nome,");
            query.AppendLine("turma.ano,");
            query.AppendLine("tipo_ciclo.id,");
            query.AppendLine("tipo_ciclo.descricao,");
            query.AppendLine("rpr.nome,");
            query.AppendLine("o.nome,");
            query.AppendLine("e.descricao,");
            query.AppendLine("o.ordem,");
            query.AppendLine("tipo_ciclo.descricao,");
            query.AppendLine("e.id,");
            query.AppendLine("o.id,");
            query.AppendLine("rpr.id");
            query.AppendLine("order by");
            query.AppendLine("o.ordem,");
            query.AppendLine("rpr.ordem;");

            var parametros = new { dreId, ueId, cicloId, turmaId, ano, pagina, periodoId, anoLetivo };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return await conexao.QueryAsync<RetornoResumoPAPTotalResultadoDto>(query.ToString(), parametros);
            }
        }

        public async Task<RecuperacaoParalelaPeriodo> ObterPeriodoPorId(long id)
        {
            var query = @"select id, nome, descricao, bimestre_edicao from recuperacao_paralela_periodo
                         where not excluido and id = @Id";
            var parametros = new { Id = id } ;

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return await conexao.QueryFirstOrDefaultAsync<RecuperacaoParalelaPeriodo>(query, parametros);
            }
        }

        private void MontarCamposResumo(StringBuilder query)
        {
            query.AppendLine("tipo_ciclo.descricao as ciclo,");
            query.AppendLine("count(aluno_id) as total,");
            query.AppendLine("tipo_ciclo.id as cicloId,");
            query.AppendLine("turma.ano,");
            query.AppendLine("tipo_ciclo.descricao,");
            query.AppendLine("rpr.nome as resposta,");
            query.AppendLine("o.descricao as objetivo,");
            query.AppendLine("e.descricao as eixo,");
            query.AppendLine("e.id as eixoId,");
            query.AppendLine("o.id as objetivoId,");
            query.AppendLine("rpr.id as respostaId,");
            query.AppendLine("rpr.ordem as ordem");
        }
        private void MontarFromResumo(StringBuilder query)
        {
            query.AppendLine("from recuperacao_paralela rp");
            query.AppendLine("inner join turma on rp.turma_id = turma.id");
            query.AppendLine("inner join tipo_ciclo_ano tca on turma.modalidade_codigo = tca.modalidade and turma.ano = tca.ano");
            query.AppendLine("inner join tipo_ciclo on tca.tipo_ciclo_id = tipo_ciclo.id");
            query.AppendLine("inner join recuperacao_paralela_periodo_objetivo_resposta rpp on rp.id = rpp.recuperacao_paralela_id");
            query.AppendLine("inner join recuperacao_paralela_resposta rpr on rpp.resposta_id = rpr.id");
            query.AppendLine("inner join recuperacao_paralela_objetivo o on rpp.objetivo_id = o.id");
            query.AppendLine("inner join recuperacao_paralela_eixo e on o.eixo_id = e.id");
            query.AppendLine("inner join  ue on ue.id = turma.ue_id");
            query.AppendLine("inner join dre on dre.id = ue.dre_id");
        }

        private void MontarWhereRecuperacaoParalela(StringBuilder query, string dreId, string ueId, int? cicloId, string ano, int? periodoId, string turmaId, int? pagina)
        {
            query.AppendLine(" where rp.excluido = false ");
            if (!string.IsNullOrEmpty(dreId) && dreId != "0")
                query.AppendLine(" and dre.dre_id = @dreId");
            if (!string.IsNullOrEmpty(ueId) && ueId != "0")
                query.AppendLine(" and ue.ue_id = @ueId");
            if (cicloId != null && cicloId != 0)
                query.AppendLine(" and tca.tipo_ciclo_id = @cicloId");
            if (!string.IsNullOrEmpty(ano) && ano != "0")
                query.AppendLine("and turma.ano = @ano");
            if (periodoId != null && periodoId != 0)
                query.AppendLine(" and rpp.periodo_recuperacao_paralela_id = @periodoId");
            if (!string.IsNullOrEmpty(turmaId) && turmaId != "0")
                query.AppendLine(" and turma.turma_id = @turmaId");
            if (pagina.HasValue)
                query.AppendLine(" and o.pagina = @pagina");

            query.AppendLine(" and rp.ano_letivo = @anoLetivo");

        }
    }
}
