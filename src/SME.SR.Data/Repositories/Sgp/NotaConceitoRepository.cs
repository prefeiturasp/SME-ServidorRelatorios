using Dapper;
using Npgsql;
using Polly;
using Polly.Registry;
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
        private readonly IAsyncPolicy policy;

        public NotaConceitoRepository(VariaveisAmbiente variaveisAmbiente, IReadOnlyPolicyRegistry<string> registry)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
            this.policy = registry.Get<IAsyncPolicy>(PoliticaPolly.PublicaFila);
        }

        public async Task<IEnumerable<NotasAlunoBimestre>> ObterNotasTurmasAlunos(string[] codigosAluno, string[] codigosTurmas, int anoLetivo, int modalidade, int semestre)
        {
            var query = @"  --> Notas bimestrais
                            select distinct t.turma_id CodigoTurma,
                                    cccat.aluno_codigo CodigoAluno,
                                    cccatn.componente_curricular_id CodigoComponenteCurricular,        
	                                coalesce(cccatn.bimestre, 0) as Bimestre,       
                                    pe.periodo_inicio PeriodoInicio,
                                    pe.periodo_fim PeriodoFim,
                                    cccatn.id NotaId,	                                   
                                    cccatn.conceito_id ConceitoId,
                                    cv.valor Conceito,
	                                cccatn.nota	                                  
                            from consolidado_conselho_classe_aluno_turma_nota cccatn
                            join consolidado_conselho_classe_aluno_turma cccat on cccat.id = cccatn.consolidado_conselho_classe_aluno_turma_id 
                            join componente_curricular cc on cc.id = cccatn.componente_curricular_id 
                            join turma t on t.id = cccat.turma_id 
                            join fechamento_turma ft on ft.turma_id = t.id 
                            join periodo_escolar pe on ft.periodo_escolar_id = pe.id and pe.bimestre = cccatn.bimestre 
                            left join conceito_valores cv on cccatn.conceito_id = cv.id
                            where t.ano_letivo = @anoLetivo 
                                  and t.modalidade_codigo = @modalidade 
                                  and t.semestre = @semestre 
                                  and t.turma_id = any(@codigosTurmas) 
                                  and cccat.aluno_codigo = any(@codigosAluno) 
                                  and not ft.excluido      
                            group by t.turma_id,
                                    cccat.aluno_codigo,
                                    cccatn.componente_curricular_id,        
	                               coalesce(cccatn.bimestre, 0),       
                                    pe.periodo_inicio,
                                    pe.periodo_fim,
                                    cccatn.conceito_id,
                                    cv.valor,
	                               cccatn.nota,
	                               cccatn.id   
                            Union all
                            --> Notas finais (sem bimestres)
                            select distinct t.turma_id,
                                    cccat.aluno_codigo,
                                    cccatn.componente_curricular_id,        
	                                0 Bimestre,       
                                    null::date periodo_inicio,
                                    null::date periodo_fim,
                                    cccatn.id NotaId,	                                   
                                    cccatn.conceito_id ConceitoId,
                                    cv.valor Conceito,
	                                cccatn.nota
                            from consolidado_conselho_classe_aluno_turma_nota cccatn
                            join consolidado_conselho_classe_aluno_turma cccat on cccat.id = cccatn.consolidado_conselho_classe_aluno_turma_id 
                            join componente_curricular cc on cc.id = cccatn.componente_curricular_id 
                            join turma t on t.id = cccat.turma_id 
                            join fechamento_turma ft on ft.turma_id = t.id and ft.periodo_escolar_id is null
                            left join conceito_valores cv on cccatn.conceito_id = cv.id
                            where t.ano_letivo = @anoLetivo 
                                  and t.modalidade_codigo = @modalidade 
                                  and t.semestre = @semestre 
                                  and t.turma_id = any(@codigosTurmas) 
                                  and cccat.aluno_codigo = any(@codigosAluno) 
                                  and not ft.excluido 
                                  and coalesce(cccatn.bimestre, 0) = 0
                            group by t.turma_id,
                                    cccat.aluno_codigo,
                                    cccatn.componente_curricular_id,        
                                    cccatn.conceito_id,
                                    cv.valor,
	                               cccatn.nota,
	                               cccatn.id 
                            order by CodigoAluno, bimestre,CodigoComponenteCurricular ";
            

            var parametros = new
            {
                codigosAluno,
                codigosTurmas,
                anoLetivo,
                modalidade,
                semestre
            };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return await policy.ExecuteAsync(() => conexao.QueryAsync<NotasAlunoBimestre, PeriodoEscolar,
                                                NotaConceitoBimestreComponente, NotasAlunoBimestre>(query.ToString()
                    , (notasFrequenciaAlunoBimestre, periodoEscolar, notaConceito) =>
                    {
                        notasFrequenciaAlunoBimestre.PeriodoEscolar = periodoEscolar;
                        notasFrequenciaAlunoBimestre.NotaConceito = notaConceito;

                        return notasFrequenciaAlunoBimestre;
                    }
                    , parametros, splitOn: "CodigoTurma,Bimestre,NotaId"));
            }
        }

        public async Task<IEnumerable<NotasAlunoBimestre>> ObterNotasTurmasAlunosParaAtaFinalAsync(string[] codigosAlunos, int anoLetivo, int[] modalidade, int semestre, int[] tiposTurma)
        {
            var query = @"select t.id as IdTurma, t.turma_id as CodigoTurma, t.tipo_turma as TipoTurma, cccat.aluno_codigo as CodigoAluno,
                           cccatn.componente_curricular_id CodigoComponenteCurricular, coalesce(ccp.aprovado, false) as Aprovado,
                           cccatn.id as NotaId, cccatn.conceito_id as ConceitoId, coalesce(cccatn.bimestre, 0) as Bimestre, 
                           cv.valor as Conceito, cccatn.nota as Nota
                           from turma t 
                           inner join consolidado_conselho_classe_aluno_turma cccat on t.id = cccat.turma_id   
                           inner join consolidado_conselho_classe_aluno_turma_nota cccatn on cccatn.consolidado_conselho_classe_aluno_turma_id = cccat.id 
                           left join conselho_classe_parecer ccp on ccp.id = cccat.parecer_conclusivo_id 
                           left join conceito_valores cv on cv.id = cccatn.conceito_id 
                           where not cccat.excluido 
                             and cccat.aluno_codigo = any(@codigosAlunos)
                             and t.ano_letivo = @anoLetivo 
                             and t.modalidade_codigo = any(@modalidade) 
                             and t.semestre = @semestre";

            if (tiposTurma.Length > 0)
                query += " and t.tipo_turma = any(@tiposTurma)";

            var parametros = new
            {
                codigosAlunos,
                anoLetivo,
                modalidade = modalidade.Length > 0 ? modalidade : null,
                semestre,
                tiposTurma = tiposTurma.Length > 0 ? tiposTurma : null
            };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryAsync<NotasAlunoBimestre, NotaConceitoBimestreComponente, NotasAlunoBimestre>(query
                    , (notasFrequenciaAlunoBimestre, notaConceito) =>
                    {
                        notasFrequenciaAlunoBimestre.NotaConceito = notaConceito;

                        return notasFrequenciaAlunoBimestre;
                    }
                    , parametros, splitOn: "CodigoTurma,NotaId", commandTimeout:6000);
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
                         inner join fechamento_turma_disciplina ftd on ftd.fechamento_turma_id = ft.id and not ftd.excluido
                         inner join fechamento_aluno fa on fa.fechamento_turma_disciplina_id = ftd.id and not fa.excluido
                         inner join fechamento_nota fn on fn.fechamento_aluno_id = fa.id and not fn.excluido
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
                         inner join conselho_classe cc on cc.fechamento_turma_id = ft.id and not cc.excluido
                         inner join conselho_classe_aluno cca on cca.conselho_classe_id  = cc.id and not cca.excluido
                         inner join conselho_classe_nota ccn on ccn.conselho_classe_aluno_id = cca.id and not ccn.excluido
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

            var query = $@"(
                            {queryRegular}
                            union 
                            {queryComplementar}
                           )";

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
