using Dapper;
using Npgsql;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class ConselhoClasseConsolidadoRepository : IConselhoClasseConsolidadoRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public ConselhoClasseConsolidadoRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<IEnumerable<ConselhoClasseConsolidadoTurmaAlunoDto>> ObterConselhosClasseConsolidadoPorTurmasAsync(string[] turmasCodigo)
        {
            var query = new StringBuilder(@" select c.id, c.dt_atualizacao DataAtualizacao, c.status, c.aluno_codigo AlunoCodigo, 
                                                    c.parecer_conclusivo_id ParecerConclusivoId, t.turma_id TurmaCodigo, c.bimestre
                            from consolidado_conselho_classe_aluno_turma c
                            inner join turma t on c.turma_id = t.id
                          where not c.excluido 
                            and t.turma_id = ANY(@turmasCodigo) ");


            var parametros = new { turmasCodigo};

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp);
            return await conexao.QueryAsync<ConselhoClasseConsolidadoTurmaAlunoDto>(query.ToString(), parametros);
        }

        public async Task<IEnumerable<ConselhoClasseConsolidadoTurmaAlunoDto>> ObterConselhosClasseConsolidadoPorTurmasTodasUesAsync(string[] turmasCodigo)
        {
            var query = new StringBuilder(@" select 		
	                                           t.turma_id TurmaCodigo,
	                                           u.nome as NomeUe,
	                                           t.nome as NomeTurma,
	                                           cccat.bimestre,
	                                           t.modalidade_codigo as ModalidadeCodigo,
	                                           'Não Iniciado: '||(count(cccat.id) filter(where cccat.status = 0)
	                                           ||', Em Andamento: '||count(cccat.id) filter(where cccat.status = 1)
	                                           ||', Concluído: ' ||count(cccat.id) filter(where cccat.status = 2)) as SomatoriaStatus
                                           from consolidado_conselho_classe_aluno_turma cccat
	                                           inner join turma t 
	                                               on t.id = cccat.turma_id
	                                           inner join ue u
	                                               on u.id = t.ue_id
                                           where t.turma_id  = ANY(@turmasCodigo)
	                                           and not cccat.excluido
	                                           group  by t.turma_id,u.nome,t.nome,cccat.bimestre,t.modalidade_codigo;");


            var parametros = new { turmasCodigo };

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp);
            return await conexao.QueryAsync<ConselhoClasseConsolidadoTurmaAlunoDto>(query.ToString(), parametros);
        }
    }
}
