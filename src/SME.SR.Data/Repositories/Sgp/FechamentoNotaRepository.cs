using Dapper;
using Npgsql;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class FechamentoNotaRepository : IFechamentoNotaRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public FechamentoNotaRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<IEnumerable<NotaConceitoBimestreComponente>> ObterNotasAlunoBimestre(long fechamentoTurmaId, string codigoAluno)
        {
            var query = FechamentoNotaConsultas.NotasAlunoBimestre;
            var parametros = new { FechamentoTurmaId = fechamentoTurmaId, CodigoAluno = codigoAluno};

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return await conexao.QueryAsync<NotaConceitoBimestreComponente>(query, parametros);
            }
        }

        public async Task<IEnumerable<HistoricoAlteracaoNotasDto>> ObterHistoricoAlteracaoNotasFechamento(long turmaId)
        {
            var query = @"select fa.aluno_codigo as codigoAluno,
	                             hn.nota_anterior as notaAnterior,
	                             hn.nota_nova as notaAtribuida,
	                             hn.conceito_anterior_id as conceitoAnteriorId,
	                             hn.conceito_novo_id as conceitoAtribuidoId, 
	                             hn.criado_por as usuarioAlteracao,
	                             hn.criado_rf as RfAlteracao,
	                             hn.criado_em as DataAlteracao	   
                            from historico_nota hn 
                           inner join historico_nota_fechamento hnf on hn.id = hnf.historico_nota_id 
                           inner join fechamento_nota fn on hnf.fechamento_nota_id = fn.id  
                           inner join fechamento_aluno fa on fn.fechamento_aluno_id = fa.id 
                           inner join fechamento_turma_disciplina ftd on fa.fechamento_turma_disciplina_id = ftd.id 
                           inner join fechamento_turma ft on ftd.fechamento_turma_id = ft.id 
                           where ft.turma_id = @turmaId";

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return await conexao.QueryAsync<HistoricoAlteracaoNotasDto>(query, new { turmaId });
            }
        }
    }
}
