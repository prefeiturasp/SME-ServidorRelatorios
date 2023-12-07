using Dapper;
using Npgsql;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class ConselhoClasseNotaRepository : IConselhoClasseNotaRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public ConselhoClasseNotaRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<IEnumerable<NotaConceitoBimestreComponente>> ObterNotasAluno(long conselhoClasseId, string codigoAluno)
        {
            var query = @"select 
                             ccn.id, ccn.componente_curricular_codigo as ComponenteCurricularCodigo, 
                             ccn.conceito_id as ConceitoId, cv.valor as Conceito, ccn.nota
                          from conselho_classe_aluno cca 
                         inner join conselho_classe_nota ccn on ccn.conselho_classe_aluno_id = cca.id
                         left join conceito_valores cv on ccn.conceito_id = cv.id
                          where cca.aluno_codigo = @codigoAluno
                            and cca.conselho_classe_id = @conselhoClasseId";

            var parametros = new { ConselhoClasseId = conselhoClasseId, CodigoAluno = codigoAluno };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryAsync<NotaConceitoBimestreComponente>(query, parametros);
            }
        }

        public async Task<IEnumerable<NotaConceitoBimestreComponente>> ObterNotasFinaisAlunoBimestre(string codigoTurma, string codigoAluno)
        {
            var query = @"select distinct * from (
                select pe.bimestre, fn.disciplina_id as ComponenteCurricularCodigo, 
                coalesce(ccn.conceito_id, fn.conceito_id) as ConceitoId, 
                coalesce(cvc.valor, cvf.valor) as Conceito, 
                coalesce(ccn.nota, fn.nota) as Nota
                  from fechamento_turma ft
                  left join periodo_escolar pe on pe.id = ft.periodo_escolar_id 
                 inner join turma t on t.id = ft.turma_id 
                 inner join fechamento_turma_disciplina ftd on ftd.fechamento_turma_id = ft.id
                 inner join fechamento_aluno fa on fa.fechamento_turma_disciplina_id = ftd.id
                 inner join fechamento_nota fn on fn.fechamento_aluno_id = fa.id
                 left join conceito_valores cvf on fn.conceito_id = cvf.id
                 inner join conselho_classe cc on cc.fechamento_turma_id = ft.id
                  left join conselho_classe_aluno cca on cca.conselho_classe_id  = cc.id
		                                        and cca.aluno_codigo = fa.aluno_codigo 
                  left join conselho_classe_nota ccn on ccn.conselho_classe_aluno_id = cca.id 
		                                        and ccn.componente_curricular_codigo = fn.disciplina_id 
                  left join conceito_valores cvc on ccn.conceito_id = cvc.id
                 where t.turma_id = @codigoTurma
                   and fa.aluno_codigo = @codigoAluno
                union all 
                select pe.bimestre, ccn.componente_curricular_codigo as ComponenteCurricularCodigo, 
                       coalesce(ccn.conceito_id, fn.conceito_id) as ConceitoId, 
                       coalesce(cvc.valor, cvf.valor) as Conceito, 
                       coalesce(ccn.nota, fn.nota) as Nota
                  from fechamento_turma ft
                  left join periodo_escolar pe on pe.id = ft.periodo_escolar_id 
                 inner join turma t on t.id = ft.turma_id 
                 inner join conselho_classe cc on cc.fechamento_turma_id = ft.id
                 inner join conselho_classe_aluno cca on cca.conselho_classe_id  = cc.id
                 inner join conselho_classe_nota ccn on ccn.conselho_classe_aluno_id = cca.id 
                  left join conceito_valores cvc on ccn.conceito_id = cvc.id
                  left join fechamento_turma_disciplina ftd on ftd.fechamento_turma_id = ft.id
                  left join fechamento_aluno fa on fa.fechamento_turma_disciplina_id = ftd.id
		                                        and cca.aluno_codigo = fa.aluno_codigo 
                  left join fechamento_nota fn on fn.fechamento_aluno_id = fa.id
		                                        and ccn.componente_curricular_codigo = fn.disciplina_id 
                  left join conceito_valores cvf on fn.conceito_id = cvf.id
                 where t.turma_id = @codigoTurma
                   and cca.aluno_codigo = @codigoAluno
                ) x	";

            var parametros = new { CodigoAluno = codigoAluno, CodigoTurma = codigoTurma };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryAsync<NotaConceitoBimestreComponente>(query, parametros);
            }
        }

        public async Task<IEnumerable<NotaConceitoBimestreComponente>> ObterNotasFinaisPorTurma(string turmaCodigo)
        {
            var query = @"select distinct * from (
                select fa.aluno_codigo as AlunoCodigo
                	, pe.bimestre
                	, fn.disciplina_id as ComponenteCurricularCodigo
                	, coalesce(ccn.conceito_id, fn.conceito_id) as ConceitoId
                	, coalesce(cvc.valor, cvf.valor) as Conceito
                	, coalesce(ccn.nota, fn.nota) as Nota
                    , sv.valor as Sintese
                  from fechamento_turma ft
                  left join periodo_escolar pe on pe.id = ft.periodo_escolar_id 
                 inner join turma t on t.id = ft.turma_id 
                 inner join fechamento_turma_disciplina ftd on ftd.fechamento_turma_id = ft.id
                 inner join fechamento_aluno fa on fa.fechamento_turma_disciplina_id = ftd.id
                 inner join fechamento_nota fn on fn.fechamento_aluno_id = fa.id
                 left join conceito_valores cvf on fn.conceito_id = cvf.id
                 inner join conselho_classe cc on cc.fechamento_turma_id = ft.id
                  left join conselho_classe_aluno cca on cca.conselho_classe_id  = cc.id
		                                        and cca.aluno_codigo = fa.aluno_codigo 
                  left join conselho_classe_nota ccn on ccn.conselho_classe_aluno_id = cca.id 
		                                        and ccn.componente_curricular_codigo = fn.disciplina_id 
                  left join conceito_valores cvc on ccn.conceito_id = cvc.id
                  left join sintese_valores sv on sv.id = fn.sintese_id
                 where t.turma_id = @turmaCodigo 
                union all 
                select cca.aluno_codigo as AlunoCodigo
                	, pe.bimestre
                	, ccn.componente_curricular_codigo as ComponenteCurricularCodigo
                	, coalesce(ccn.conceito_id, fn.conceito_id) as ConceitoId
                	, coalesce(cvc.valor, cvf.valor) as Conceito
                	, coalesce(ccn.nota, fn.nota) as Nota
                    , null as Sintese
                  from fechamento_turma ft
                  left join periodo_escolar pe on pe.id = ft.periodo_escolar_id 
                 inner join turma t on t.id = ft.turma_id 
                 inner join conselho_classe cc on cc.fechamento_turma_id = ft.id
                 inner join conselho_classe_aluno cca on cca.conselho_classe_id  = cc.id
                 inner join conselho_classe_nota ccn on ccn.conselho_classe_aluno_id = cca.id 
                  left join conceito_valores cvc on ccn.conceito_id = cvc.id
                  left join fechamento_turma_disciplina ftd on ftd.fechamento_turma_id = ft.id
                  left join fechamento_aluno fa on fa.fechamento_turma_disciplina_id = ftd.id
		                                        and cca.aluno_codigo = fa.aluno_codigo 
                  left join fechamento_nota fn on fn.fechamento_aluno_id = fa.id
		                                        and ccn.componente_curricular_codigo = fn.disciplina_id 
                  left join conceito_valores cvf on fn.conceito_id = cvf.id
                 where t.turma_id = @turmaCodigo 
                ) x	";

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas);

            return await conexao.QueryAsync<NotaConceitoBimestreComponente>(query, new { turmaCodigo });
        }

        public async Task<IEnumerable<RetornoNotaConceitoBimestreComponenteDto>> ObterNotasFinaisRelatorioNotasConceitosFinais(string[] dresCodigos, string[] uesCodigos, int? semestre, int modalidade, string[] anos, int anoLetivo, int[] bimestres, long[] componentesCurricularesCodigos)
        {
            var query = new StringBuilder();
            query.AppendLine("WITH tmpNotasFinais AS (");            
            query.AppendLine("select coalesce(ccn.id, fn.id ) AS id,");
            query.AppendLine("       wanf.id AS workFlowAprovacaoId,");
            query.AppendLine("       fa.aluno_codigo AS AlunoCodigo,");
            query.AppendLine("       pe.bimestre,");
            query.AppendLine("       fn.disciplina_id AS ComponenteCurricularCodigo,");
            query.AppendLine("       coalesce(ccn.conceito_id, fn.conceito_id) AS ConceitoId,");
            query.AppendLine("       coalesce(cvc.valor, cvf.valor) AS Conceito,");
            query.AppendLine("       coalesce(ccn.nota, fn.nota) AS Nota,"); 
            query.AppendLine($@"      
		                             (  
                                        SELECT COALESCE(wanf.nota::varchar, cvwf.valor)
		                                FROM wf_aprovacao_nota_fechamento wanf
		                                LEFT JOIN conceito_valores cvwf ON wanf.conceito_id = cvwf.id
		                                WHERE fn.id = wanf.fechamento_nota_id AND NOT wanf.excluido
		                             ) AS notaConceitoEmAprovacao,");
            query.AppendLine($@"     (
		                                SELECT COALESCE(wanc.nota::varchar, cvwc.valor)
		                                FROM wf_aprovacao_nota_conselho wanc
		                                LEFT JOIN conceito_valores cvwc ON wanc.conceito_id = cvwc.id
		                                WHERE ccn.id = wanc.conselho_classe_nota_id AND NOT wanc.excluido
		                             ) AS notaConceitoPosConselhoEmAprovacao,");
            query.AppendLine("       ccn.id AS conselhoClasseNotaId,");
            query.AppendLine("       CASE WHEN ccn.nota IS NOT NULL");
            query.AppendLine("           OR ccn.conceito_id IS NOT NULL THEN");
            query.AppendLine("           0");
            query.AppendLine("       ELSE");
            query.AppendLine("           1");
            query.AppendLine("       END EhNotaConceitoFechamento,");
            query.AppendLine("       cca.id ConselhoClasseAlunoId,");
            query.AppendLine("       CASE WHEN EXISTS (");
            query.AppendLine("           SELECT 1 ");
            query.AppendLine("           FROM");
            query.AppendLine("                conselho_classe_aluno_turma_complementar");
            query.AppendLine("           WHERE");
            query.AppendLine("                conselho_classe_aluno_id = cca.id) THEN 1");
            query.AppendLine("        ELSE");
            query.AppendLine("            0");
            query.AppendLine("        END PossuiTurmaAssociada,");
            query.AppendLine("        sv.id AS SinteseId,");
            query.AppendLine("        sv.valor AS Sintese,");
            query.AppendLine("        dre.nome AS dreNome,");
            query.AppendLine("        dre.dre_id AS dreCodigo,");
            query.AppendLine("        dre.abreviacao AS dreAbreviacao,");
            query.AppendLine("        ue.ue_id AS ueCodigo,");
            query.AppendLine("        ue.nome AS ueNome,");
            query.AppendLine("        ue.tipo_escola AS tipoEscola,");
            query.AppendLine("        t.ano,");
            query.AppendLine("        t.turma_id AS turmaCodigo,");
            query.AppendLine("        t.nome AS turmaNome");
            query.AppendLine("	from turma t ");
            query.AppendLine("		inner join ue");
            query.AppendLine("			on t.ue_id = ue.id");
            query.AppendLine("		inner join dre");
            query.AppendLine("			on ue.dre_id = dre.id");
            query.AppendLine("		inner join fechamento_turma ft ");
            query.AppendLine("			on t.id = ft.turma_id");
            query.AppendLine("		inner join fechamento_turma_disciplina ftd ");
            query.AppendLine("			on ft.id = ftd.fechamento_turma_id");
            query.AppendLine("		inner join fechamento_aluno fa ");
            query.AppendLine("			on ftd.id = fa.fechamento_turma_disciplina_id");
            query.AppendLine("		inner join fechamento_nota fn ");
            query.AppendLine("			on fa.id = fn.fechamento_aluno_id");
            query.AppendLine("		left join periodo_escolar pe ");
            query.AppendLine("			on ft.periodo_escolar_id = pe.id");
            query.AppendLine("		left join conceito_valores cvf");
            query.AppendLine("			on fn.conceito_id = cvf.id");
            query.AppendLine("		left join conselho_classe cc ");
            query.AppendLine("			on ft.id = cc.fechamento_turma_id and not cc.excluido");
            query.AppendLine("		left join conselho_classe_aluno cca ");
            query.AppendLine("			on cc.id = cca.conselho_classe_id and not cca.excluido");
            query.AppendLine("		left join conselho_classe_nota ccn ");
            query.AppendLine("			on cca.id = ccn.conselho_classe_aluno_id and not ccn.excluido");
            query.AppendLine("		left join conceito_valores cvc");
            query.AppendLine("			on ccn.conceito_id = cvc.id");
            query.AppendLine("		left join sintese_valores sv");
            query.AppendLine("			on fn.sintese_id = sv.id");
            query.AppendLine("		left join wf_aprovacao_nota_fechamento wanf");
            query.AppendLine("			on fn.id = wanf.fechamento_nota_id and not wanf.excluido");

            query.AppendLine($"where {(bimestres.Contains(0) ? "(pe.bimestre is null or" : "(")} pe.bimestre = ANY(@bimestres)) and");
            if (modalidade > 0)
                query.AppendLine("       t.modalidade_codigo = @modalidade and");
            if (anos != null && anos.Length > 0)
                query.AppendLine("	     t.ano = any(@anos) and");
            if (anoLetivo > 0)
                query.AppendLine("	     t.ano_letivo = @anoLetivo and");
            if (dresCodigos != null && dresCodigos.Length > 0)
                query.AppendLine("	     dre.dre_id = ANY(@dresCodigos) and");
            if (uesCodigos != null && uesCodigos.Length > 0)
                query.AppendLine("       ue.ue_id = ANY(@uesCodigos) and");
            if (semestre != null && semestre > 0)
                query.AppendLine(@" t.semestre = @semestre and");
            if (componentesCurricularesCodigos != null && componentesCurricularesCodigos.Length > 0)
                query.AppendLine(@" fn.disciplina_id = ANY(@componentesCurricularesCodigos) and");
            query.AppendLine("       not ft.excluido and");
            query.AppendLine("       not ftd.excluido and");
            query.AppendLine("       not fa.excluido and");
            query.AppendLine("       not fn.excluido");            
            query.AppendLine("UNION ALL");
            query.AppendLine("select coalesce(ccn.id, fn.id) AS id,");
            query.AppendLine("       wanf.id AS workFlowAprovacaoId,");
            query.AppendLine("       cca.aluno_codigo AS AlunoCodigo,");
            query.AppendLine("       pe.bimestre,");
            query.AppendLine("       ccn.componente_curricular_codigo AS ComponenteCurricularCodigo,");
            query.AppendLine("       coalesce(ccn.conceito_id, fn.conceito_id) AS ConceitoId,");
            query.AppendLine("       coalesce(cvc.valor, cvf.valor) AS Conceito,");
            query.AppendLine("       coalesce(ccn.nota, fn.nota) AS Nota,");
            query.AppendLine($@"      
		                             (  
                                        SELECT COALESCE(wanf.nota::varchar, cvwf.valor)
		                                FROM wf_aprovacao_nota_fechamento wanf
		                                LEFT JOIN conceito_valores cvwf ON wanf.conceito_id = cvwf.id
		                                WHERE fn.id = wanf.fechamento_nota_id AND NOT wanf.excluido
		                             ) AS notaConceitoEmAprovacao,");
            query.AppendLine($@"     (
		                                SELECT COALESCE(wanc.nota::varchar, cvwc.valor)
		                                FROM wf_aprovacao_nota_conselho wanc
		                                LEFT JOIN conceito_valores cvwc ON wanc.conceito_id = cvwc.id
		                                WHERE ccn.id = wanc.conselho_classe_nota_id AND NOT wanc.excluido
		                             ) AS notaConceitoPosConselhoEmAprovacao,");
            query.AppendLine("       ccn.id AS conselhoClasseNotaId,");
            query.AppendLine("       case when ccn.nota is not null or ccn.conceito_id IS NOT NULL ");
            query.AppendLine("          then 0");
            query.AppendLine("	      else 1");
            query.AppendLine("	     end EhNotaConceitoFechamento,");
            query.AppendLine("       cca.id ComponenteCurricularAlunoId,");
            query.AppendLine("       case when exists (select 1");
            query.AppendLine("	        		         from conselho_classe_aluno_turma_complementar");
            query.AppendLine("        				  where conselho_classe_aluno_id = cca.id) ");
            query.AppendLine("	     then 1");
            query.AppendLine("         else 0");
            query.AppendLine("       end PossuiTurmaAssociada,");
            query.AppendLine("       NULL AS SinteseId,");
            query.AppendLine("       NULL AS Sintese,");
            query.AppendLine("       dre.nome AS dreNome,");
            query.AppendLine("       dre.dre_id AS dreCodigo,");
            query.AppendLine("       dre.abreviacao AS dreAbreviacao,");
            query.AppendLine("       ue.ue_id AS ueCodigo,");
            query.AppendLine("       ue.nome AS ueNome,");
            query.AppendLine("       ue.tipo_escola AS tipoEscola,");
            query.AppendLine("       t.ano,");
            query.AppendLine("       t.turma_id AS turmaCodigo,");
            query.AppendLine("       t.nome AS turmaNome");
            query.AppendLine("  	from turma t");
            query.AppendLine("  		inner join ue");
            query.AppendLine("  			on t.ue_id = ue.id");
            query.AppendLine("  		inner join dre");
            query.AppendLine("  			on ue.dre_id = dre.id");
            query.AppendLine("  		inner join fechamento_turma ft ");
            query.AppendLine("  			on t.id = ft.turma_id ");
            query.AppendLine("  		inner join conselho_classe cc ");
            query.AppendLine("  			on ft.id = cc.fechamento_turma_id");
            query.AppendLine("  		inner join conselho_classe_aluno cca ");
            query.AppendLine("  			on cc.id = cca.conselho_classe_id");
            query.AppendLine("  		inner join conselho_classe_nota ccn");
            query.AppendLine("  			on cca.id = ccn.conselho_classe_aluno_id");
            query.AppendLine("  		left join periodo_escolar pe ");
            query.AppendLine("  			on ft.periodo_escolar_id = pe.id");
            query.AppendLine("  		left join conceito_valores cvc");
            query.AppendLine("  			on ccn.conceito_id = cvc.id");
            query.AppendLine("  		left join fechamento_turma_disciplina ftd");
            query.AppendLine("  			on ft.id = ftd.fechamento_turma_id and not ftd.excluido");
            query.AppendLine("  		left join fechamento_aluno fa");
            query.AppendLine("  			on ftd.id = ftd.fechamento_turma_id and not fa.excluido");
            query.AppendLine("  		left join fechamento_nota fn ");
            query.AppendLine("  			on fa.fechamento_turma_disciplina_id = fn.id and not fn.excluido");
            query.AppendLine("  		left join conceito_valores cvf");
            query.AppendLine("  			on fn.conceito_id = cvf.id");
            query.AppendLine("  		left join wf_aprovacao_nota_fechamento wanf ");
            query.AppendLine("  			on fn.id = wanf.fechamento_nota_id and not wanf.excluido");
            
            query.AppendLine($"where {(bimestres.Contains(0) ? "(pe.bimestre is null or " : "(")} pe.bimestre = ANY(@bimestres)) and");
            if (anoLetivo > 0)
                query.AppendLine("     t.ano_letivo = @anoLetivo and");
            if (anos != null && anos.Length > 0)
                query.AppendLine(" 	   t.ano = any(@anos) and");
            if (modalidade > 0)
                query.AppendLine(" 	   t.modalidade_codigo = @modalidade and");
            if (dresCodigos != null && dresCodigos.Length > 0)
                query.AppendLine(" 	   dre.dre_id = any(@dresCodigos) and");
            if (uesCodigos != null && uesCodigos.Length > 0)
                query.AppendLine(" 	   ue.ue_id = any(@uesCodigos) and");
            if (semestre != null && semestre > 0)
                query.AppendLine(@" t.semestre = @semestre and");
            if (componentesCurricularesCodigos != null && componentesCurricularesCodigos.Length > 0)
                query.AppendLine(@" fn.disciplina_id = ANY(@componentesCurricularesCodigos) and");
            query.AppendLine(" 	   not ft.excluido and");
            query.AppendLine(" 	   not cc.excluido and");
            query.AppendLine(" 	   not cca.excluido and");
            query.AppendLine(" 	   not ccn.excluido),");            
            query.AppendLine("lista AS (");
            query.AppendLine("    SELECT");
            query.AppendLine("        AlunoCodigo,");
            query.AppendLine("        bimestre,");
            query.AppendLine("        ComponenteCurricularCodigo,");
            query.AppendLine("        ConceitoId,");
            query.AppendLine("        Conceito,");
            query.AppendLine("        Nota,");
            query.AppendLine("        notaConceitoEmAprovacao,");
            query.AppendLine("        notaConceitoPosConselhoEmAprovacao,");
            query.AppendLine("        conselhoClasseNotaId,");
            query.AppendLine("        EhNotaConceitoFechamento,");
            query.AppendLine("        ConselhoClasseAlunoId,");
            query.AppendLine("        PossuiTurmaAssociada,");
            query.AppendLine("        SinteseId,");
            query.AppendLine("        Sintese,");
            query.AppendLine("        dreNome,");
            query.AppendLine("        dreCodigo,");
            query.AppendLine("        dreAbreviacao,");
            query.AppendLine("        ueCodigo,");
            query.AppendLine("        ueNome,");
            query.AppendLine("        tipoEscola,");
            query.AppendLine("        ano,");
            query.AppendLine("        turmaCodigo,");
            query.AppendLine("        turmaNome,");
            query.AppendLine("        Row_number() OVER (PARTITION BY AlunoCodigo,");
            query.AppendLine("            bimestre,");
            query.AppendLine("            ComponenteCurricularCodigo,");
            query.AppendLine("            turmaCodigo ORDER BY Id,");
            query.AppendLine("            workFlowAprovacaoId DESC) AS sequencia");
            query.AppendLine("FROM");
            query.AppendLine("    tmpNotasFinais");
            query.AppendLine(")");
            query.AppendLine("SELECT");
            query.AppendLine("    *");
            query.AppendLine("FROM");
            query.AppendLine("    lista");
            query.AppendLine("WHERE");
            query.AppendLine("    sequencia = 1;");           

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas);
            return await conexao.QueryAsync<RetornoNotaConceitoBimestreComponenteDto>(query.ToString(), new { bimestres, dresCodigos, uesCodigos, semestre, modalidade, anos, anoLetivo, componentesCurricularesCodigos }, commandTimeout: 300);
        }

        public async Task<IEnumerable<HistoricoAlteracaoNotasDto>> ObterHistoricoAlteracaoNotasConselhoClasse(long turmaId, long tipocalendarioId, int[] bimestres, long[] componentes)
        {
            var query = new StringBuilder(@$"select distinct cca.aluno_codigo as codigoAluno,
                                 hn.nota_anterior as notaAnterior,
                                 hn.nota_nova as notaAtribuida,
                                 hn.conceito_anterior_id as conceitoAnteriorId,
                                 hn.conceito_novo_id as conceitoAtribuidoId, 
                                 hn.criado_por as usuarioAlteracao,
                                 3 as TipoNota,
                                 hn.criado_rf as rfAlteracao,
                                 hn.criado_em as dataAlteracao,
                                 ftd.disciplina_id as disciplinaId,
                                 wanc.id is not null as EmAprovacao,
                                 pe.bimestre,
                                 coalesce(cc2.descricao_sgp,cc2.descricao) as componentecurricularNome
                              from historico_nota hn
                             inner join historico_nota_conselho_classe hncc on hn.id = hncc.historico_nota_id
                             inner join conselho_classe_nota ccn on hncc.conselho_classe_nota_id = ccn.id 
                             inner join conselho_classe_aluno cca on ccn.conselho_classe_aluno_id = cca.id 
                             inner join conselho_classe cc on cca.conselho_classe_id = cc.id 
                             inner join fechamento_turma ft on cc.fechamento_turma_id = ft.id
                             inner join fechamento_turma_disciplina ftd on ft.id = ftd.fechamento_turma_id 
                             left join periodo_escolar pe on ft.periodo_escolar_id = pe.id
                             inner join componente_curricular cc2 on ftd.disciplina_id = cc2.id 
                             left join wf_aprovacao_nota_conselho wanc on wanc.conselho_classe_nota_id  = ccn.id
                             where ft.turma_id = @turmaId");

            if (bimestres.Contains(0))
                query.AppendLine(@" and ft.periodo_escolar_id is null ");

            if (bimestres.Contains(-99))
                query.AppendLine(@" and pe.tipo_calendario_id = @tipocalendarioId and pe.bimestre = ANY('{1,2,3,4}') ");

            if (!bimestres.Contains(-99) && !bimestres.Contains(0))
                query.AppendLine(@" and pe.tipo_calendario_id = @tipocalendarioId and pe.bimestre = ANY(@bimestres) ");

            if (componentes.Length > 0)
                query.AppendLine(@" and ftd.disciplina_id = ANY(@componentes)");

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryAsync<HistoricoAlteracaoNotasDto>(query.ToString(), new { turmaId, tipocalendarioId, bimestres, componentes });
            }
        }
    }
}
    