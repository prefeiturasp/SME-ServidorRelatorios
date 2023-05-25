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
            var query = new StringBuilder(); 
            query.AppendLine("drop table if exists tmp_lista_fechamento_conselho;");
            query.AppendLine("create temporary table tmp_lista_fechamento_conselho as");
            query.AppendLine("select distinct t.turma_id,");
            query.AppendLine("                fa.aluno_codigo,");
            query.AppendLine("                ftd.fechamento_turma_id,");
            query.AppendLine("                coalesce(ccn.componente_curricular_codigo, fn.disciplina_id) disciplina_id,");
            query.AppendLine("                ftd.id fechamento_turma_disciplina_id,");
            query.AppendLine("                pe.bimestre,");
            query.AppendLine("                pe.periodo_inicio,");
            query.AppendLine("                pe.periodo_fim,");
            query.AppendLine("                coalesce(ccn.id, fn.id) nota_id,");
            query.AppendLine("                coalesce(ccn.conceito_id, fn.conceito_id) conceito_id,");
            query.AppendLine("                coalesce(cv2.valor, cv.valor) valor_conceito,");
            query.AppendLine("                coalesce(ccn.nota, fn.nota) nota,");
            query.AppendLine("                ccn.id is not null eh_conceito");
            query.AppendLine("    from turma t");
            query.AppendLine("        inner join fechamento_turma ft");
            query.AppendLine("            on t.id = ft.turma_id");
            query.AppendLine("        inner join fechamento_turma_disciplina ftd");
            query.AppendLine("            on ft.id = ftd.fechamento_turma_id");            
            query.AppendLine("        inner join fechamento_aluno fa");
            query.AppendLine("            on ftd.id = fa.fechamento_turma_disciplina_id");            
            query.AppendLine("        left join fechamento_nota fn");
            query.AppendLine("            on fa.id = fn.fechamento_aluno_id");            
            query.AppendLine("        left join periodo_escolar pe");
            query.AppendLine("            on ft.periodo_escolar_id = pe.id");
            query.AppendLine("        left join conceito_valores cv");
            query.AppendLine("            on fn.conceito_id = cv.id");
            query.AppendLine("        left join conselho_classe cc");
            query.AppendLine("            on ft.id = cc.fechamento_turma_id and");
            query.AppendLine("               not cc.excluido");
            query.AppendLine("        left join conselho_classe_aluno cca");
            query.AppendLine("            on cc.id = cca.conselho_classe_id and");
            query.AppendLine("               fa.aluno_codigo = cca.aluno_codigo and");
            query.AppendLine("               not cca.excluido");
            query.AppendLine("        left join conselho_classe_nota ccn");
            query.AppendLine("            on cca.id = ccn.conselho_classe_aluno_id and");
            query.AppendLine("               ftd.disciplina_id = ccn.componente_curricular_codigo and");
            query.AppendLine("               not ccn.excluido");
            query.AppendLine("        left join conceito_valores cv2");
            query.AppendLine("            on ccn.conceito_id = cv2.id");
            query.AppendLine("where t.ano_letivo = @anoLetivo and");
            query.AppendLine("      t.modalidade_codigo = @modalidade and");
            query.AppendLine("      t.semestre = @semestre and");
            query.AppendLine("      t.turma_id = any(@codigosTurmas) and");
            query.AppendLine("      fa.aluno_codigo = any(@codigosAluno) and");
            query.AppendLine("      not ft.excluido;");
            query.AppendLine("drop table if exists tmp_lista_conselho_fechamento;");
            query.AppendLine("create temporary table tmp_lista_conselho_fechamento as");
            query.AppendLine("select distinct t.turma_id,");
            query.AppendLine("                cca.aluno_codigo,");
            query.AppendLine("                ccn.componente_curricular_codigo,");
            query.AppendLine("                ftd.fechamento_turma_disciplina_id,");
            query.AppendLine("                pe.bimestre,");
            query.AppendLine("                pe.periodo_inicio,");
            query.AppendLine("                pe.periodo_fim,");
            query.AppendLine("                ccn.id nota_id,");
            query.AppendLine("                ccn.conceito_id conceito_id,");
            query.AppendLine("                cv.valor valor_conceito,");
            query.AppendLine("                ccn.nota nota,");
            query.AppendLine("                true eh_conceito");
            query.AppendLine("    from conselho_classe_aluno cca");
            query.AppendLine("        inner join conselho_classe cc");
            query.AppendLine("            on cca.conselho_classe_id = cc.id");
            query.AppendLine("        inner join fechamento_turma ft");
            query.AppendLine("            on cc.fechamento_turma_id = ft.id");
            query.AppendLine("        inner join turma t");
            query.AppendLine("            on ft.turma_id = t.id");
            query.AppendLine("        inner join conselho_classe_nota ccn");
            query.AppendLine("            on cca.id = ccn.conselho_classe_aluno_id");
            query.AppendLine("        left join conceito_valores cv");
            query.AppendLine("            on ccn.conceito_id = cv.id");
            query.AppendLine("        left join periodo_escolar pe");
            query.AppendLine("            on ft.periodo_escolar_id = pe.id");
            query.AppendLine("        left join tmp_lista_fechamento_conselho ftd");
            query.AppendLine("            on ft.id = ftd.fechamento_turma_id and");
            query.AppendLine("               cca.aluno_codigo = ftd.aluno_codigo and");
            query.AppendLine("               ccn.componente_curricular_codigo = ftd.disciplina_id and");
            query.AppendLine("               pe.bimestre = ftd.bimestre");
            query.AppendLine("where t.ano_letivo = @anoLetivo and");
            query.AppendLine("      t.modalidade_codigo = @modalidade and");
            query.AppendLine("      t.semestre = @semestre and");
            query.AppendLine("      t.turma_id = any(@codigosTurmas) and");
            query.AppendLine("      cca.aluno_codigo = any(@codigosAluno) and");
            query.AppendLine("      not cca.excluido and");
            query.AppendLine("      not cc.excluido and");
            query.AppendLine("      not ft.excluido and");
            query.AppendLine("      not ccn.excluido and");
            query.AppendLine("      not ft.excluido;");
            query.AppendLine("with lista_juncao as (");
            query.AppendLine("select turma_id,");
            query.AppendLine("       aluno_codigo,");
            query.AppendLine("       disciplina_id,");
            query.AppendLine("       fechamento_turma_disciplina_id,");
            query.AppendLine("       bimestre,");
            query.AppendLine("       periodo_inicio,");
            query.AppendLine("       periodo_fim,");
            query.AppendLine("       nota_id,");
            query.AppendLine("       conceito_id,");
            query.AppendLine("       valor_conceito,");
            query.AppendLine("       nota,");
            query.AppendLine("       eh_conceito");
            query.AppendLine("    from tmp_lista_fechamento_conselho");
            query.AppendLine("union");
            query.AppendLine("select *");
            query.AppendLine("    from tmp_lista_conselho_fechamento),");
            query.AppendLine("lista_sequenciada as (");
            query.AppendLine("select *,");
            query.AppendLine("       row_number() over (partition by aluno_codigo, bimestre, disciplina_id order by eh_conceito desc, nota_id desc) sequencia");
            query.AppendLine("    from lista_juncao)");
            query.AppendLine("select turma_id CodigoTurma,");
            query.AppendLine("       aluno_codigo CodigoAluno,");
            query.AppendLine("       disciplina_id CodigoComponenteCurricular,");
            query.AppendLine("       fechamento_turma_disciplina_id fechamentoDisciplina,");
            query.AppendLine("       bimestre,");
            query.AppendLine("       periodo_inicio PeriodoInicio,");
            query.AppendLine("       periodo_fim PeriodoFim,");
            query.AppendLine("       nota_id NotaId,");
            query.AppendLine("       conceito_id ConceitoId,");
            query.AppendLine("       valor_conceito Conceito,");
            query.AppendLine("       nota Nota");
            query.AppendLine("    from lista_sequenciada a");
            query.AppendLine("        inner join componente_curricular cc");
            query.AppendLine("            on a.disciplina_id = cc.id");
            query.AppendLine("where sequencia = 1;");

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
                return await conexao.QueryAsync<NotasAlunoBimestre, PeriodoEscolar,
                                                NotaConceitoBimestreComponente, NotasAlunoBimestre>(query.ToString()
                    , (notasFrequenciaAlunoBimestre, periodoEscolar, notaConceito) =>
                    {
                        notasFrequenciaAlunoBimestre.PeriodoEscolar = periodoEscolar;
                        notasFrequenciaAlunoBimestre.NotaConceito = notaConceito;

                        return notasFrequenciaAlunoBimestre;
                    }
                    , parametros, splitOn: "CodigoTurma,Bimestre,NotaId");
            }
        }

        public async Task<IEnumerable<NotasAlunoBimestre>> ObterNotasTurmasAlunosParaAtaFinalAsync(string[] codigosAlunos, int anoLetivo, int[] modalidade, int semestre, int[] tiposTurma)
        {
            var query = "select * from f_ata_final_obter_notas_turmas_alunos(@anoLetivo, @modalidade, @semestre, @tiposTurma, @codigosAlunos)";

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
