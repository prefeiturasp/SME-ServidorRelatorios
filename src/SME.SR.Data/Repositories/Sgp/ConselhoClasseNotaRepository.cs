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

            var query = new StringBuilder(@$"with tmpNotasFinais as (
                select distinct * from (
                select coalesce(ccn.id,fn.id) as id, fa.aluno_codigo as AlunoCodigo
                	, pe.bimestre
                	, fn.disciplina_id as ComponenteCurricularCodigo
                	, coalesce(ccn.conceito_id, fn.conceito_id) as ConceitoId
                	, coalesce(cvc.valor, cvf.valor) as Conceito
                	, coalesce(ccn.nota, fn.nota) as Nota
                    , coalesce(wanf.nota::varchar, cv.valor) as notaConceitoEmAprovacao
                    , coalesce(wfnc.nota::varchar, cv1.valor) as notaConceitoPosConselhoEmAprovacao
                    , ccn.id as conselhoClasseNotaId
                    , CASE
							WHEN ccn.nota is not null OR ccn.conceito_id is not null  THEN 0
							ELSE 1
					  END EhNotaConceitoFechamento
                    , cca.id ConselhoClasseAlunoId
                    , CASE WHEN EXISTS(SELECT 1 FROM conselho_classe_aluno_turma_complementar where conselho_classe_aluno_id = cca.id) THEN 1 
                           ELSE 0 
                      END PossuiTurmaAssociada
                    , sv.id as SinteseId
                    , sv.valor as Sintese
                    , d.nome as dreNome
                    , d.dre_id as dreCodigo
                    , d.abreviacao as dreAbreviacao
                    , u.ue_id  as ueCodigo
                    , u.nome  as ueNome
                    , u.tipo_escola as tipoEscola
                    , t.ano
                    , t.turma_id as turmaCodigo
                    , t.nome as turmaNome
                  from fechamento_turma ft
                  left join periodo_escolar pe on pe.id = ft.periodo_escolar_id 
                 inner join turma t on t.id = ft.turma_id 
                 inner join ue u on t.ue_id  = u.id 
                 inner join dre d on u.dre_id = d.id   
                 inner join fechamento_turma_disciplina ftd on ftd.fechamento_turma_id = ft.id
                 inner join fechamento_aluno fa on fa.fechamento_turma_disciplina_id = ftd.id
                 inner join fechamento_nota fn on fn.fechamento_aluno_id = fa.id
                 left join conceito_valores cvf on fn.conceito_id = cvf.id
                 left join conselho_classe cc on cc.fechamento_turma_id = ft.id
                  left join conselho_classe_aluno cca on cca.conselho_classe_id  = cc.id
		                                        and cca.aluno_codigo = fa.aluno_codigo 
                  left join conselho_classe_nota ccn on ccn.conselho_classe_aluno_id = cca.id 
		                                        and ccn.componente_curricular_codigo = fn.disciplina_id 
                  left join conceito_valores cvc on ccn.conceito_id = cvc.id
                  left join sintese_valores sv on sv.id = fn.sintese_id
                  left join wf_aprovacao_nota_fechamento wanf on wanf.fechamento_nota_id = fn.id 
                  left join conceito_valores cv on cv.id = wanf.conceito_id 
                  left join wf_aprovacao_nota_conselho wfnc on wfnc.conselho_classe_nota_id = ccn.id
                  left join conceito_valores cv1 on cv1.id = wfnc.conceito_id
                 where {(bimestres.Contains(0) ? "(pe.bimestre is null or" : "(")} pe.bimestre = ANY(@bimestres)) ");

            if (dresCodigos != null && dresCodigos.Length > 0)
                query.AppendLine(@" and d.dre_id = ANY(@dresCodigos) ");

            if (uesCodigos != null && uesCodigos.Length > 0)
                query.AppendLine(@" and u.ue_id = any(@uesCodigos) ");

            if (semestre != null && semestre > 0)
                query.AppendLine(@" and t.semestre = @semestre ");

            if (modalidade > 0)
                query.AppendLine(@" and t.modalidade_codigo = @modalidade ");

            if (anos != null && anos.Length > 0)
                query.AppendLine(@" and t.ano = any(@anos) ");

            if (anoLetivo > 0)
                query.AppendLine(@" and t.ano_letivo = @anoLetivo ");

            if (componentesCurricularesCodigos != null && componentesCurricularesCodigos.Length > 0)
                query.AppendLine(@" and fn.disciplina_id = ANY(@componentesCurricularesCodigos) ");

            query.AppendLine(@$"union all 
                select coalesce(ccn.id,fn.id) as id, cca.aluno_codigo as AlunoCodigo
                	, pe.bimestre
                	, ccn.componente_curricular_codigo as ComponenteCurricularCodigo
                	, coalesce(ccn.conceito_id, fn.conceito_id) as ConceitoId
                	, coalesce(cvc.valor, cvf.valor) as Conceito
                	, coalesce(ccn.nota, fn.nota) as Nota
                    , coalesce(wanf.nota::varchar, cv.valor) as notaConceitoEmAprovacao
                    , coalesce(wfnc.nota::varchar, cv1.valor) as notaConceitoPosConselhoEmAprovacao
                    , ccn.id as conselhoClasseNotaId
                    , CASE
							WHEN ccn.nota is not null OR ccn.conceito_id is not null  THEN 0
							ELSE 1
					 END EhNotaConceitoFechamento
                    , cca.id ComponenteCurricularAlunoId
                    , CASE WHEN EXISTS(SELECT 1 FROM conselho_classe_aluno_turma_complementar where conselho_classe_aluno_id = cca.id) THEN 1 
                           ELSE 0 
                      END PossuiTurmaAssociada
                    , null as SinteseId
                    , null as Sintese
                    , d.nome as dreNome
                    , d.dre_id as dreCodigo
                    , d.abreviacao as dreAbreviacao
                    , u.ue_id  as ueCodigo
                    , u.nome  as ueNome
                    , u.tipo_escola as tipoEscola
                    , t.ano
                    , t.turma_id as turmaCodigo
                    , t.nome as turmaNome
                  from fechamento_turma ft
                  left join periodo_escolar pe on pe.id = ft.periodo_escolar_id 
                 inner join turma t on t.id = ft.turma_id 
                 inner join ue u on t.ue_id  = u.id 
                 inner join dre d on u.dre_id = d.id   
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
                  left join wf_aprovacao_nota_fechamento wanf on wanf.fechamento_nota_id = fn.id 
                  left join conceito_valores cv on cv.id = wanf.conceito_id
                  left join wf_aprovacao_nota_conselho wfnc on wfnc.conselho_classe_nota_id = ccn.id
                  left join conceito_valores cv1 on cv1.id = wfnc.conceito_id
                 where {(bimestres.Contains(0) ? "(pe.bimestre is null or " : "(")} pe.bimestre = ANY(@bimestres)) ");

            if (dresCodigos != null && dresCodigos.Length > 0)
                query.AppendLine(@" and d.dre_id = ANY(@dresCodigos) ");

            if (uesCodigos != null && uesCodigos.Length > 0)
                query.AppendLine(@" and u.ue_id = any(@uesCodigos) ");

            if (semestre != null && semestre > 0)
                query.AppendLine(@" and t.semestre = @semestre ");

            if (modalidade > 0)
                query.AppendLine(@" and t.modalidade_codigo = @modalidade ");

            if (anos != null && anos.Length > 0)
                query.AppendLine(@" and t.ano = any(@anos) ");

            if (anoLetivo > 0)
                query.AppendLine(@" and t.ano_letivo = @anoLetivo ");

            if (componentesCurricularesCodigos != null && componentesCurricularesCodigos.Length > 0)
                query.AppendLine(@" and ccn.componente_curricular_codigo = ANY(@componentesCurricularesCodigos) ");

            query.AppendLine(@") x 
                ), lista as (
                select AlunoCodigo
	                   ,bimestre
                       ,ComponenteCurricularCodigo      
                       ,ConceitoId
	                   ,Conceito
                ,		Nota
                ,notaConceitoEmAprovacao
                ,notaConceitoPosConselhoEmAprovacao
                ,conselhoClasseNotaId
                ,EhNotaConceitoFechamento
                ,ConselhoClasseAlunoId
                ,PossuiTurmaAssociada
                ,SinteseId
                ,Sintese
                ,dreNome
                ,dreCodigo
                ,dreAbreviacao
                ,ueCodigo
                ,ueNome
                ,tipoEscola
                ,ano
                ,turmaCodigo
                ,turmaNome
                ,Row_number() OVER (partition by AlunoCodigo order by Id desc) as sequencia
                from tmpNotasFinais)
                select * from lista
                where sequencia = 1");

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas);
            return await conexao.QueryAsync<RetornoNotaConceitoBimestreComponenteDto>(query.ToString(), new { bimestres, dresCodigos, uesCodigos, semestre, modalidade, anos, anoLetivo, componentesCurricularesCodigos });
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
    