using Npgsql;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class ConselhoClasseConsolidadoRepository : IConselhoClasseConsolidadoRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public ConselhoClasseConsolidadoRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<IEnumerable<ConselhoClasseConsolidadoTurmaAlunoDto>> ObterConselhosClasseConsolidadoPorTurmasAsync(string[] turmasCodigo)
        {
            var query = new StringBuilder(@" select distinct 
	                                                c.id,
	                                                c.dt_atualizacao DataAtualizacao,
	                                                c.status,
	                                                c.aluno_codigo AlunoCodigo,
	                                                c.parecer_conclusivo_id ParecerConclusivoId,
	                                                t.turma_id TurmaCodigo,
	                                                cccatn.bimestre
                                                from
	                                                consolidado_conselho_classe_aluno_turma c inner join turma t on c.turma_id = t.id
                                                inner join consolidado_conselho_classe_aluno_turma_nota cccatn on cccatn.consolidado_conselho_classe_aluno_turma_id = c.id
                                                where not c.excluido 
                                                and t.turma_id = ANY(@turmasCodigo) ");

            var parametros = new { turmasCodigo };

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas);
            return await conexao.QueryAsync<ConselhoClasseConsolidadoTurmaAlunoDto>(query.ToString(), parametros);
        }

        public async Task<IEnumerable<ConselhoClasseConsolidadoTurmaDto>> ObterConselhosClasseConsolidadoPorTurmasTodasUesAsync(string dreCodigo, int modalidade, int[] bimestres, SituacaoConselhoClasse? situacao, int anoLetivo, int semestre, bool exibirHistorico)
        {
            var query = new StringBuilder(@" select distinct
	                                            u.ue_id as UeCodigo,
	                                            t.turma_id TurmaCodigo,
	                                            u.nome as NomeUe,
	                                            t.nome as NomeTurma,
	                                            cccatn.bimestre,
	                                            t.modalidade_codigo as ModalidadeCodigo,
	                                            count(cccat.id) filter(where cccat.status = 0) as NaoIniciado,
	                                            count(cccat.id) filter(where cccat.status = 1) as EmAndamento,
	                                            count(cccat.id) filter(where cccat.status = 2) as Concluido
                                            from
	                                            consolidado_conselho_classe_aluno_turma cccat
                                            inner join consolidado_conselho_classe_aluno_turma_nota cccatn on cccatn.consolidado_conselho_classe_aluno_turma_id = cccat.id
                                            inner join turma t on t.id = cccat.turma_id
                                            inner join ue u on u.id = t.ue_id
                                            inner join dre d on d.id = u.dre_id
                                            where t.ano_letivo =@anoLetivo
                                              and d.dre_id = @dreCodigo
                                              and t.modalidade_codigo = @modalidade
                                              and not cccat.excluido ");

            if (bimestres != null)
                query.AppendLine(" and cccatn.bimestre = ANY(@bimestres) ");

            if (semestre > 0)
                query.AppendLine(" and t.semestre = @semestre ");

            if (situacao != null)
                query.AppendLine(" and cccat.status = @situacao ");

            if (!exibirHistorico)
                query.AppendLine(" and not t.historica ");

            query.AppendLine(@" group by u.ue_id, t.turma_id, t.id, u.nome, t.nome, cccatn.bimestre, t.modalidade_codigo
                                order by u.nome, t.nome, cccatn.bimestre;");

            var parametros = new { dreCodigo, modalidade, bimestres, situacao, anoLetivo, semestre };

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp);
            return await conexao.QueryAsync<ConselhoClasseConsolidadoTurmaDto>(query.ToString(), parametros);

        }

        public async Task<IEnumerable<NotasAlunoBimestreBoletimSimplesDto>> ObterNotasBoletimPorAlunoTurma(string[] alunosCodigos, string[] turmasCodigos, int semestre)
        {
            var query = new StringBuilder(@"select cccat.turma_id CodigoTurma, cccat.aluno_codigo CodigoAluno,
                                 cccatn.componente_curricular_id CodigoComponenteCurricular,
                                 coalesce(cccatn.bimestre, 0) as Bimestre, coalesce((cast (cccatn.nota as varchar)),cv.valor) as NotaConceito
                              from consolidado_conselho_classe_aluno_turma cccat 
                              inner join consolidado_conselho_classe_aluno_turma_nota cccatn on cccatn.consolidado_conselho_classe_aluno_turma_id = cccat.id 
                               left join conceito_valores cv on cv.id = cccatn.conceito_id 
                              inner join turma t on t.id = cccat.turma_id");

            bool passaPorWhere = alunosCodigos != null || turmasCodigos != null || semestre > 0;

            if (passaPorWhere)
            {
                query.AppendLine(" where");
                if (alunosCodigos != null)
                    query.AppendLine(" cccat.aluno_codigo = ANY(@alunosCodigos) ");

                if (turmasCodigos != null)
                    query.AppendLine(" and t.turma_id = ANY(@turmasCodigos)");

                if (semestre > 0)
                    query.AppendLine(" and t.semestre = @semestre");
            }

            query.AppendLine("and exists (select 1");
            query.AppendLine("    from conselho_classe cc");
            query.AppendLine("   		inner join fechamento_turma ft");
            query.AppendLine("   			on cc.fechamento_turma_id = ft.id");
            query.AppendLine("   		inner join periodo_escolar pe");
            query.AppendLine("   			on ft.periodo_escolar_id = pe.id");
            query.AppendLine("   		inner join fechamento_turma_disciplina ftd");
            query.AppendLine("   			on ft.id = ftd.fechamento_turma_id");
            query.AppendLine("   		inner join fechamento_aluno fa");
            query.AppendLine("   			on ftd.id = fa.fechamento_turma_disciplina_id");
            query.AppendLine("   		inner join fechamento_nota fn");
            query.AppendLine("   			on fa.id = fn.fechamento_aluno_id");
            query.AppendLine(" where ft.turma_id = t.id");
            query.AppendLine(" 	and pe.bimestre = cccatn.bimestre");
            query.AppendLine(" 	and fa.aluno_codigo = cccat.aluno_codigo");
            query.AppendLine(" 	and fn.disciplina_id = cccatn.componente_curricular_id");
            query.AppendLine("	and not cc.excluido");
            query.AppendLine("	and not ft.excluido");
            query.AppendLine("	and not ftd.excluido");
            query.AppendLine("	and not fa.excluido");
            query.AppendLine("	and not fn.excluido");
            query.AppendLine("union");
            query.AppendLine("select 1");
            query.AppendLine("	from conselho_classe cc2");
            query.AppendLine("		inner join fechamento_turma ft2");
            query.AppendLine("			on cc2.fechamento_turma_id = ft2.id");
            query.AppendLine("		inner join conselho_classe_aluno cca");
            query.AppendLine("			on cc2.id = cca.conselho_classe_id");
            query.AppendLine("		inner join conselho_classe_nota ccn");
            query.AppendLine("			on cca.id = ccn.conselho_classe_aluno_id");
            query.AppendLine("		inner join periodo_escolar pe2");
            query.AppendLine("			on ft2.periodo_escolar_id = pe2.id");
            query.AppendLine("where ft2.turma_id = t.id");
            query.AppendLine(" 	and pe2.bimestre = cccatn.bimestre");
            query.AppendLine(" 	and cca.aluno_codigo = cccat.aluno_codigo");
            query.AppendLine(" 	and ccn.componente_curricular_codigo = cccatn.componente_curricular_id");
            query.AppendLine("	and not cca.excluido");
            query.AppendLine("	and not ccn.excluido);");

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas);
            var parametros = new { alunosCodigos, turmasCodigos, semestre };
            try
            {
                return await conexao.QueryAsync<NotasAlunoBimestreBoletimSimplesDto>(query.ToString(), parametros);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
