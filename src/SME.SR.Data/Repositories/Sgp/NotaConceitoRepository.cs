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
            var query = @"select t.turma_id CodigoTurma, fa.aluno_codigo CodigoAluno,
                                 fn.disciplina_id CodigoComponenteCurricular,
                                 ftd.id fechamentoDisciplina,
                                 pe.bimestre, pe.periodo_inicio PeriodoInicio,
                                 pe.periodo_fim PeriodoFim, fn.id NotaId,
                                 coalesce(ccn.conceito_id, fn.conceito_id) as ConceitoId, 
                                 coalesce(cv1.valor, cv2.valor) as Conceito, coalesce(ccn.nota, fn.nota) as Nota
	                         from turma t
	 	                        inner join fechamento_turma ft
	 		                        on t.id = ft.turma_id
	 	                        inner join fechamento_turma_disciplina ftd
	 		                        on ft.id = ftd.fechamento_turma_id and not ftd.excluido
	 	                        inner join fechamento_aluno fa
	 		                        on ftd.id = fa.fechamento_turma_disciplina_id
	 	                        inner join fechamento_nota fn
	 		                        on fa.id = fn.fechamento_aluno_id
	 	                        left join periodo_escolar pe
	 		                        on ft.periodo_escolar_id = pe.id
	 	                        left join conceito_valores cv1
	 		                        on fn.conceito_id = cv1.id
	 	                        left join conselho_classe cc
	 		                        on ft.id = cc.fechamento_turma_id
		                        left join conselho_classe_aluno cca
			                        on cc.id = cca.conselho_classe_id and
			                           fa.aluno_codigo = cca.aluno_codigo
		                        left join conselho_classe_nota ccn
			                        on cca.id = ccn.conselho_classe_aluno_id and
			                           fn.disciplina_id = ccn.componente_curricular_codigo
		                        left join conceito_valores cv2
			                        on ccn.conceito_id = cv2.id
                         where t.ano_letivo = @anoLetivo
 	                        and t.modalidade_codigo = @modalidade
 	                        and t.semestre = @semestre
 	                        and fa.aluno_codigo = any(@codigosAluno)
                            and t.turma_id = any(@codigosTurmas)
 
                         union
                          
                         select t.turma_id CodigoTurma, cca.aluno_codigo CodigoAluno,
                                ccn.componente_curricular_codigo CodigoComponenteCurricular,
                                f.fechamentoDisciplina,
                                pe.bimestre, pe.periodo_inicio PeriodoInicio,
                                pe.periodo_fim PeriodoFim, ccn.id NotaId,
                                coalesce(ccn.conceito_id, f.conceito_id) as ConceitoId, 
   		                        coalesce(cv1.valor, f.valor) as Conceito, coalesce(ccn.nota, f.nota) as Nota
	                        from conselho_classe_aluno cca 
		                        inner join conselho_classe cc
			                        on cca.conselho_classe_id = cc.id
		                        inner join fechamento_turma ft
			                        on cc.fechamento_turma_id = ft.id
		                        inner join turma t
			                        on ft.turma_id = t.id
		                        inner join conselho_classe_nota ccn
			                        on cca.id = ccn.conselho_classe_aluno_id
		                        left join periodo_escolar pe
 			                        on ft.periodo_escolar_id = pe.id
 		                        left join conceito_valores cv1
 			                        on ccn.conceito_id = cv1.id
 		                        left join (select ftd.fechamento_turma_id,
                                                  ftd.id fechamentoDisciplina,
 						                          fa.aluno_codigo,
 						                          fn.disciplina_id,
 						                          fn.conceito_id,
 						                          cv2.valor,
 						                          fn.nota
 				                              from fechamento_turma_disciplina ftd
 				   	                             inner join fechamento_aluno fa
 				   	     	                        on ftd.id = fa.fechamento_turma_disciplina_id and not ftd.excluido
 				   	                             inner join fechamento_nota fn
 				   	     	                        on fa.id = fn.fechamento_aluno_id
 				   	                             inner join conceito_valores cv2
 				   	     	                        on fn.conceito_id = cv2.id
 				                           where fa.aluno_codigo = any(@codigosAluno)) f
 			                        on ft.id = f.fechamento_turma_id and
 			                           cca.aluno_codigo = f.aluno_codigo and
 			                           ccn.componente_curricular_codigo = f.disciplina_id
                        where cca.aluno_codigo = any(@codigosAluno)
	                        and t.ano_letivo = @anoLetivo
	                        and t.modalidade_codigo = @modalidade
 	                        and t.semestre = @semestre
                            and t.turma_id = any(@codigosTurmas);";

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

        public async Task<IEnumerable<NotasAlunoBimestre>> ObterNotasTurmasAlunosParaAtaFinalAsync(string[] codigosAlunos, string codigoTurma, int anoLetivo, int modalidade, int semestre, int[] tiposTurma)
        {
            var query = "select * from f_ata_final_obter_notas_turmas_alunos(@anoLetivo, @modalidade, @semestre, @tiposTurma, @codigosAlunos)";

            var parametros = new
            {
                codigosAlunos,
                codigoTurma,
                anoLetivo,
                modalidade,
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
