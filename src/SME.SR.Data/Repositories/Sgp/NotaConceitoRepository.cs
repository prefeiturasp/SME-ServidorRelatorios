using Dapper;
using Npgsql;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
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

        public async Task<IEnumerable<NotasAlunoBimestre>> ObterNotasTurmasAlunos(string[] codigosTurma, string[] codigosAluno)
        {

            var query = @"select distinct * from (
                        select t.turma_id CodigoTurma, fa.aluno_codigo CodigoAluno,
                               fn.disciplina_id CodigoComponenteCurricular,
                               pe.bimestre, pe.periodo_inicio PeriodoInicio,
                               pe.periodo_fim PeriodoFim, coalesce(ccn.conceito_id, fn.conceito_id) as ConceitoId, 
                               coalesce(cvc.valor, cvf.valor) as Conceito, coalesce(ccn.nota, fn.nota) as Nota
                          from fechamento_turma ft
                         inner join periodo_escolar pe on pe.id = ft.periodo_escolar_id 
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
                         where t.turma_id = ANY(@codigosTurma)
                           and fa.aluno_codigo = ANY(@codigosAluno)
                        union all 
                        select t.turma_id CodigoTurma, fa.aluno_codigo CodigoAluno,
                               fn.disciplina_id CodigoComponenteCurricular,
                               pe.bimestre, pe.periodo_inicio PeriodoInicio,
                               pe.periodo_fim PeriodoFim, coalesce(ccn.conceito_id, fn.conceito_id) as ConceitoId, 
                               coalesce(cvc.valor, cvf.valor) as Conceito, coalesce(ccn.nota, fn.nota) as Nota
                          from fechamento_turma ft
                          inner join periodo_escolar pe on pe.id = ft.periodo_escolar_id 
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
                         where t.turma_id = ANY(@codigosTurma)
                           and cca.aluno_codigo = ANY(@codigosAluno)
                        ) x ";

            var parametros = new
            {
                CodigosTurma = codigosTurma,
                CodigosAluno = codigosAluno
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
                    , parametros, splitOn: "CodigoTurma,Bimestre,ConceitoId, TotalAulas");
            }
        }
    }
}
