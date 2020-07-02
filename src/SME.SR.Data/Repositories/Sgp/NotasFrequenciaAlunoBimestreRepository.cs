using Dapper;
using Npgsql;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class NotasFrequenciaAlunoBimestreRepository : INotasFrequenciaAlunoBimestreRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public NotasFrequenciaAlunoBimestreRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<IEnumerable<NotasFrequenciaAlunoBimestre>> ObterNotasFrequenciaAlunosBimestre(string[] codigosTurma, string[] codigosAluno)
        {
            var query = @"select ft.turma_id CodigoTurma, cca.aluno_codigo CodigoAluno,
	                           ccn.componente_curricular_codigo CodigoComponenteCurricular,
	                           pe.bimestre, pe.periodo_inicio PeriodoInicio,
	                           pe.periodo_fim PeriodoFim, ccn.conceito_id as ConceitoId, 
	                           cv.valor as Conceito, ccn.nota, fqa.total_aulas TotalAulas,
	                           fqa.total_ausencias TotalAusencias, fqa.total_compensacoes TotalCompensacoes
                        from fechamento_turma ft 
                        inner join periodo_escolar pe on ft.periodo_escolar_id = pe.id 
                        inner join conselho_classe cc on ft.id = cc.fechamento_turma_id 
                        inner join conselho_classe_aluno cca on cc.id = cca.conselho_classe_id 
                        inner join conselho_classe_nota ccn on cca.id = ccn.conselho_classe_aluno_id 
                        left join conceito_valores cv on ccn.conceito_id = cv.id
                        left join frequencia_aluno fqa on ft.turma_id::varchar = fqa.turma_id and
							                               cca.aluno_codigo = fqa.codigo_aluno and 
								                           ft.periodo_escolar_id = fqa.periodo_escolar_id and 
								                           ccn.componente_curricular_codigo::varchar = fqa.disciplina_id and 
								                           fqa.tipo = 1
                        where ft.turma_id in @codigosTurma and cca.aluno_codigo in @codigosAluno ";

            var parametros = new
            {
                CodigosTurma = codigosTurma,
                CodigosAluno = codigosAluno
            };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return await conexao.QueryAsync<NotasFrequenciaAlunoBimestre>(query, parametros);
            }
        }
    }
}
