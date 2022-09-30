using Dapper;
using Npgsql;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class NotaConceitoRepository : INotaConceitoRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public NotaConceitoRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<IEnumerable<NotasAlunoBimestre>> ObterNotasTurmasAlunos(string[] codigosAluno, string[] codigosTurmas, int anoLetivo, int modalidade, int semestre)
        {
            var query = @"drop table if exists tmp_lista_fechamento_conselho;
                            create temporary table tmp_lista_fechamento_conselho as
                            select distinct t.turma_id,
	   			                            fa.aluno_codigo,
	   			                            ftd.fechamento_turma_id,
	   			                            fn.disciplina_id,	   			
	   			                            ftd.id fechamento_turma_disciplina_id,
	   			                            pe.bimestre,
	   			                            pe.periodo_inicio,
	   			                            pe.periodo_fim,
	   			                            coalesce(ccn.id, fn.id) nota_id,
	   			                            coalesce(ccn.conceito_id, fn.conceito_id) conceito_id,
	   			                            coalesce(cv2.valor, cv.valor) valor_conceito,
	   			                            coalesce(ccn.nota, fn.nota) nota
	                            from turma t 
		                            inner join fechamento_turma ft 
			                            on t.id = ft.turma_id 
		                            inner join fechamento_turma_disciplina ftd 
			                            on ft.id = ftd.fechamento_turma_id and
			                               not ftd.excluido
		                            inner join fechamento_aluno fa 
			                            on ftd.id = fa.fechamento_turma_disciplina_id and
			                               not fa.excluido
		                            left join fechamento_nota fn 
			                            on fa.id = fn.fechamento_aluno_id and
			                               not fn.excluido
		                            inner join periodo_escolar pe
			                            on ft.periodo_escolar_id = pe.id
		                            left join conceito_valores cv 
			                            on fn.conceito_id = cv.id
		                            left join conselho_classe cc 
			                            on ft.id = cc.fechamento_turma_id and
			                               not cc.excluido
		                            left join conselho_classe_aluno cca 
			                            on cc.id = cca.conselho_classe_id and
			                               fa.aluno_codigo = cca.aluno_codigo and
			                               not cca.excluido			   
		                            left join conselho_classe_nota ccn 
			                            on cca.id = ccn.conselho_classe_aluno_id and
			                               ftd.disciplina_id = ccn.componente_curricular_codigo and
			                               not ccn.excluido
		                            left join conceito_valores cv2 
			                            on ccn.conceito_id = cv2.id
                            where t.ano_letivo = 2022 and
	                              t.modalidade_codigo = 5 and
	                              t.semestre = 0 and
	                              t.turma_id = any(@codigosTurmas) and
	                              fa.aluno_codigo = any(@codigosAluno) and
	                              not ft.excluido;
 
                            drop table if exists tmp_lista_conselho_fechamento;
                            create temporary table tmp_lista_conselho_fechamento as
                            select distinct t.turma_id,
	      		                            cca.aluno_codigo,
	   			                            ccn.componente_curricular_codigo,
	   			                            ftd.fechamento_turma_disciplina_id,
	   			                            pe.bimestre,
	   			                            pe.periodo_inicio,
	   			                            pe.periodo_fim,
	   			                            ccn.id nota_id,
	   			                            ccn.conceito_id conceito_id,
	   			                            cv.valor valor_conceito,
	   			                            ccn.nota nota
	                            from conselho_classe_aluno cca 
		                            inner join conselho_classe cc 
			                            on cca.conselho_classe_id = cc.id 
		                            inner join fechamento_turma ft
			                            on cc.fechamento_turma_id = ft.id
		                            inner join turma t
			                            on ft.turma_id = t.id 
		                            inner join conselho_classe_nota ccn 
			                            on cca.id = ccn.conselho_classe_aluno_id			
		                            left join conceito_valores cv 
			                            on ccn.conceito_id = cv.id
		                            inner join periodo_escolar pe 
			                            on ft.periodo_escolar_id = pe.id
		                            left join tmp_lista_fechamento_conselho ftd
			                            on ft.id = ftd.fechamento_turma_id and
			                               cca.aluno_codigo = ftd.aluno_codigo and
			                               ccn.componente_curricular_codigo = ftd.disciplina_id and
			                               pe.bimestre = ftd.bimestre			   
                            where t.ano_letivo = 2022 and
	                              t.modalidade_codigo = 5 and
	                              t.semestre = 0 and
	                              t.turma_id = any(@codigosTurmas) and
	                              cca.aluno_codigo = any(@codigosAluno) and
	                              not cca.excluido and
	                              not cc.excluido and
	                              not ft.excluido and
	                              not ccn.excluido and
	                              not ft.excluido;

                            with lista_juncao as (
                            select turma_id,
	                               aluno_codigo,
	                               disciplina_id,	   			
	                               fechamento_turma_disciplina_id,
	                               bimestre,
	                               periodo_inicio,
	                               periodo_fim,
	                               nota_id,
	                               conceito_id,
	                               valor_conceito,
	                               nota
	                            from tmp_lista_fechamento_conselho
                            union
                            select *
	                            from tmp_lista_conselho_fechamento),
                            lista_sequenciada as (
                            select *,
	                               row_number() over (partition by aluno_codigo, bimestre, disciplina_id order by nota_id) sequencia
	                            from lista_juncao)
                            select turma_id CodigoTurma,
	                               aluno_codigo CodigoAluno,
	                               disciplina_id CodigoComponenteCurricular,	   			
	                               fechamento_turma_disciplina_id fechamentoDisciplina,
	                               bimestre,
	                               periodo_inicio PeriodoInicio,
	                               periodo_fim PeriodoFim,
	                               nota_id NotaId,
	                               conceito_id ConceitoId,
	                               valor_conceito Conceito,
	                               nota Nota,	   
	                               cc.descricao_sgp,
	                               bimestre
	                            from lista_sequenciada a
		                            inner join componente_curricular cc
			                            on a.disciplina_id = cc.id
                            where sequencia = 1;";

            var parametros = new
            {
                codigosAluno,
                codigosTurmas,
                anoLetivo,
                modalidade,
                semestre
            };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryAsync<NotasAlunoBimestre, PeriodoEscolar,
                                                NotaConceitoBimestreComponente, NotasAlunoBimestre>(query
                    , (notasFrequenciaAlunoBimestre, periodoEscolar, notaConceito) =>
                    {
                        notasFrequenciaAlunoBimestre.PeriodoEscolar = periodoEscolar;
                        notasFrequenciaAlunoBimestre.NotaConceito = notaConceito;

                        return notasFrequenciaAlunoBimestre;
                    }
                    , parametros, splitOn: "CodigoTurma,Bimestre,NotaId");
            }
        }

        public async Task<IEnumerable<NotasAlunoBimestre>> ObterNotasTurmasAlunosParaAtaFinalAsync(string[] codigosAlunos, string codigoTurma, int anoLetivo, int[] modalidade, int semestre, int[] tiposTurma)
        {
            var query = "select * from f_ata_final_obter_notas_turmas_alunos(@anoLetivo, @modalidade, @semestre, @tiposTurma, @codigosAlunos)";

            var parametros = new
            {
                codigosAlunos,
                codigoTurma,
                anoLetivo,
                modalidade = modalidade.Length > 0 ? modalidade : null,
                semestre,
                tiposTurma = tiposTurma.Length > 0 ? tiposTurma : null
            };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryAsync<NotasAlunoBimestre, PeriodoEscolar,
                                                NotaConceitoBimestreComponente, NotasAlunoBimestre>(query
                    , (notasFrequenciaAlunoBimestre, periodoEscolar, notaConceito) =>
                    {
                        notasFrequenciaAlunoBimestre.PeriodoEscolar = periodoEscolar;
                        notasFrequenciaAlunoBimestre.NotaConceito = notaConceito;

                        return notasFrequenciaAlunoBimestre;
                    }
                    , parametros, splitOn: "CodigoTurma,Bimestre,NotaId", commandTimeout:6000);
            }
        }
        public async Task<IEnumerable<NotasAlunoBimestre>> ObterNotasTurmasAlunosParaAtaBimestralAsync(string[] codigosAlunos, int anoLetivo, int modalidade, int semestre, int[] tiposTurma, int bimestre)
        {
            var query = "select * from f_ata_bimestral_obter_notas_turmas_alunos(@anoLetivo, @modalidade, @semestre, @tiposTurma, @codigosAlunos, @bimestre)";

            var parametros = new
            {
                codigosAlunos,
                anoLetivo,
                modalidade,
                semestre,
                tiposTurma = tiposTurma.Length > 0 ? tiposTurma : null,
                bimestre
            };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryAsync<NotasAlunoBimestre, PeriodoEscolar,
                                                NotaConceitoBimestreComponente, NotasAlunoBimestre>(query
                    , (notasFrequenciaAlunoBimestre, periodoEscolar, notaConceito) =>
                    {
                        notasFrequenciaAlunoBimestre.PeriodoEscolar = periodoEscolar;
                        notasFrequenciaAlunoBimestre.NotaConceito = notaConceito;

                        return notasFrequenciaAlunoBimestre;
                    }
                    , parametros, splitOn: "CodigoTurma,Bimestre,NotaId");
            }
        }

        public async Task<IEnumerable<NotasAlunoBimestre>> ObterNotasTurmasAlunosParaHistoricoEscolasAsync(string[] codigosAluno, int anoLetivo, int modalidade, int semestre)
        {
            const string queryNotasRegular = @"
                        select t.turma_id CodigoTurma, fa.aluno_codigo CodigoAluno,
                               fn.disciplina_id CodigoComponenteCurricular,
                               coalesce(ccp.aprovado, false) as Aprovado,
                               coalesce(pe.bimestre,0) as bimestre, pe.periodo_inicio PeriodoInicio,
                               pe.periodo_fim PeriodoFim, fn.id NotaId,
                               coalesce(ccn.conceito_id, fn.conceito_id) as ConceitoId, 
                               coalesce(cvc.valor, cvf.valor) as Conceito, coalesce(ccn.nota, fn.nota) as Nota
                          from fechamento_turma ft
                         left join periodo_escolar pe on pe.id = ft.periodo_escolar_id 
                         inner join turma t on t.id = ft.turma_id 
                         inner join fechamento_turma_disciplina ftd on ftd.fechamento_turma_id = ft.id
                         inner join fechamento_aluno fa on fa.fechamento_turma_disciplina_id = ftd.id
                         inner join fechamento_nota fn on fn.fechamento_aluno_id = fa.id
                         left join conceito_valores cvf on fn.conceito_id = cvf.id
                         left join conselho_classe cc on cc.fechamento_turma_id = ft.id
                         left join conselho_classe_aluno cca on cca.conselho_classe_id  = cc.id and cca.aluno_codigo = fa.aluno_codigo 
                         left join conselho_classe_parecer ccp on cca.conselho_classe_parecer_id  = ccp.id   
                         left join conselho_classe_nota ccn on ccn.conselho_classe_aluno_id = cca.id and ccn.componente_curricular_codigo = fn.disciplina_id 
                         left join conceito_valores cvc on ccn.conceito_id = cvc.id
                         where fa.aluno_codigo = ANY(@codigosAluno)
                           and t.ano_letivo <= @anoLetivo ";

            const string queryNotasComplementar = @"
                        select t.turma_id CodigoTurma, cca.aluno_codigo CodigoAluno,
                               ccn.componente_curricular_codigo CodigoComponenteCurricular,
                               coalesce(ccp.aprovado, false) as Aprovado,
                               coalesce(pe.bimestre,0) as bimestre, pe.periodo_inicio PeriodoInicio,
                               pe.periodo_fim PeriodoFim, ccn.id NotaId,
                               coalesce(ccn.conceito_id, fn.conceito_id) as ConceitoId, 
                               coalesce(cvc.valor, cvf.valor) as Conceito, coalesce(ccn.nota, fn.nota) as Nota
                          from fechamento_turma ft
                          left join periodo_escolar pe on pe.id = ft.periodo_escolar_id 
                         inner join turma t on t.id = ft.turma_id 
                         inner join conselho_classe cc on cc.fechamento_turma_id = ft.id
                         inner join conselho_classe_aluno cca on cca.conselho_classe_id  = cc.id
                         inner join conselho_classe_nota ccn on ccn.conselho_classe_aluno_id = cca.id 
                         left join conselho_classe_parecer ccp on cca.conselho_classe_parecer_id  = ccp.id   
                          left join conceito_valores cvc on ccn.conceito_id = cvc.id
                          left join fechamento_turma_disciplina ftd on ftd.fechamento_turma_id = ft.id
                          left join fechamento_aluno fa on fa.fechamento_turma_disciplina_id = ftd.id
		                                                and cca.aluno_codigo = fa.aluno_codigo 
                          left join fechamento_nota fn on fn.fechamento_aluno_id = fa.id
		                                                and ccn.componente_curricular_codigo = fn.disciplina_id 
                          left join conceito_valores cvf on fn.conceito_id = cvf.id
                         where cca.aluno_codigo = ANY(@codigosAluno)
                           and t.ano_letivo <= @anoLetivo ";

            var queryRegular = new StringBuilder(queryNotasRegular);
            var queryComplementar = new StringBuilder(queryNotasComplementar);

            if (modalidade > 0)
            {
                queryRegular.AppendLine(" and t.modalidade_codigo = @modalidade ");
                queryComplementar.AppendLine(" and t.modalidade_codigo = @modalidade ");
            }

            if (semestre > 0)
            {
                queryRegular.AppendLine(" and t.semestre = @semestre ");
                queryComplementar.AppendLine(" and t.semestre = @semestre ");
            }

            var query = $@"select distinct * from (
                            {queryRegular}
                            union all 
                            {queryComplementar}
                           ) x";

            var parametros = new
            {
                codigosAluno,
                anoLetivo,
                modalidade,
                semestre
            };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryAsync<NotasAlunoBimestre, PeriodoEscolar,
                                                NotaConceitoBimestreComponente, NotasAlunoBimestre>(query
                    , (notasFrequenciaAlunoBimestre, periodoEscolar, notaConceito) =>
                    {
                        notasFrequenciaAlunoBimestre.PeriodoEscolar = periodoEscolar;
                        notasFrequenciaAlunoBimestre.NotaConceito = notaConceito;

                        return notasFrequenciaAlunoBimestre;
                    }
                    , parametros, splitOn: "CodigoTurma,Bimestre,NotaId");
            }
        }        
    }
}
