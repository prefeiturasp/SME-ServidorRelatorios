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

        public async Task<IEnumerable<NotasAlunoBimestre>> ObterNotasTurmasAlunos(string[] codigosAluno, int anoLetivo, int modalidade, int semestre)
        {
            var query = @"select distinct * from (
                        select t.turma_id CodigoTurma, fa.aluno_codigo CodigoAluno,
                               fn.disciplina_id CodigoComponenteCurricular,
                               pe.bimestre, pe.periodo_inicio PeriodoInicio,
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
                          left join conselho_classe_aluno cca on cca.conselho_classe_id  = cc.id
		                                                and cca.aluno_codigo = fa.aluno_codigo 
                          left join conselho_classe_nota ccn on ccn.conselho_classe_aluno_id = cca.id 
		                                                and ccn.componente_curricular_codigo = fn.disciplina_id 
                          left join conceito_valores cvc on ccn.conceito_id = cvc.id
                         where fa.aluno_codigo = ANY(@codigosAluno)
                           and t.ano_letivo = @anoLetivo
                           and t.modalidade_codigo = @modalidade
                           and t.semestre = @semestre
                        union all 
                        select t.turma_id CodigoTurma, cca.aluno_codigo CodigoAluno,
                               ccn.componente_curricular_codigo CodigoComponenteCurricular,
                               pe.bimestre, pe.periodo_inicio PeriodoInicio,
                               pe.periodo_fim PeriodoFim, ccn.id NotaId,
                               coalesce(ccn.conceito_id, fn.conceito_id) as ConceitoId, 
                               coalesce(cvc.valor, cvf.valor) as Conceito, coalesce(ccn.nota, fn.nota) as Nota
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
                         where cca.aluno_codigo = ANY(@codigosAluno)
                           and t.ano_letivo = @anoLetivo
                           and t.modalidade_codigo = @modalidade
                           and t.semestre = @semestre
                        ) x ";

            var parametros = new
            {
                codigosAluno,
                anoLetivo,
                modalidade,
                semestre
            };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
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

        public async Task<IEnumerable<NotasAlunoBimestre>> ObterNotasTurmasAlunosParaAtaFinalAsync(string[] codigosAlunos, int anoLetivo, int modalidade, int semestre, int[] tiposTurma)
        {
            const string queryNotasRegular = @"
                        select t.turma_id CodigoTurma, t.tipo_turma as TipoTurma, ccatc.turma_id TurmaComplementarId, fa.aluno_codigo CodigoAluno,
                               fn.disciplina_id CodigoComponenteCurricular,
                               coalesce(ccp.aprovado, false) as Aprovado,
                               coalesce(pe.bimestre,0) Bimestre,  pe.periodo_inicio PeriodoInicio,
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
                         left join conselho_classe_aluno_turma_complementar ccatc on cca.id = ccatc.conselho_classe_aluno_id
                         left join conselho_classe_parecer ccp on cca.conselho_classe_parecer_id  = ccp.id   
                         left join conselho_classe_nota ccn on ccn.conselho_classe_aluno_id = cca.id and ccn.componente_curricular_codigo = fn.disciplina_id 
                         left join conceito_valores cvc on ccn.conceito_id = cvc.id
                         where fa.aluno_codigo = ANY(@codigosAlunos)
                           and t.ano_letivo = @anoLetivo";

            const string queryNotasComplementar = @"
                        select t.turma_id CodigoTurma, t.tipo_turma as TipoTurma, ccatc.turma_id TurmaComplementarId, cca.aluno_codigo CodigoAluno,
                               ccn.componente_curricular_codigo CodigoComponenteCurricular,
                               coalesce(ccp.aprovado, false) as Aprovado,
                               coalesce(pe.bimestre,0) Bimestre, pe.periodo_inicio PeriodoInicio,
                               pe.periodo_fim PeriodoFim, ccn.id NotaId,
                               coalesce(ccn.conceito_id, fn.conceito_id) as ConceitoId, 
                               coalesce(cvc.valor, cvf.valor) as Conceito, coalesce(ccn.nota, fn.nota) as Nota
                          from fechamento_turma ft
                          left join periodo_escolar pe on pe.id = ft.periodo_escolar_id 
                         inner join conselho_classe cc on cc.fechamento_turma_id = ft.id
                         inner join conselho_classe_aluno cca on cca.conselho_classe_id  = cc.id
                         inner join conselho_classe_nota ccn on ccn.conselho_classe_aluno_id = cca.id 
                         left join conselho_classe_aluno_turma_complementar ccatc on cca.id = ccatc.conselho_classe_aluno_id
                         inner join turma t on t.id = ccatc.turma_id 
                         left join conselho_classe_parecer ccp on cca.conselho_classe_parecer_id  = ccp.id   
                          left join conceito_valores cvc on ccn.conceito_id = cvc.id
                          left join fechamento_turma_disciplina ftd on ftd.fechamento_turma_id = ft.id
                          left join fechamento_aluno fa on fa.fechamento_turma_disciplina_id = ftd.id
		                                                and cca.aluno_codigo = fa.aluno_codigo 
                          left join fechamento_nota fn on fn.fechamento_aluno_id = fa.id
		                                                and ccn.componente_curricular_codigo = fn.disciplina_id 
                          left join conceito_valores cvf on fn.conceito_id = cvf.id
                         where cca.aluno_codigo = ANY(@codigosAlunos)
                           and t.ano_letivo = @anoLetivo";

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

            if (tiposTurma.Length > 0)
            {
                queryRegular.AppendLine(" and t.tipo_turma = ANY(@tiposTurma) ");
                queryComplementar.AppendLine(" and t.tipo_turma = ANY(@tiposTurma) ");
            }

            var query = $@"select distinct * from (
                            {queryRegular}
                            union all 
                            {queryComplementar}
                           ) x";

            var parametros = new
            {
                codigosAlunos,
                anoLetivo,
                modalidade,
                semestre,
                tiposTurma
            };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
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

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
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
