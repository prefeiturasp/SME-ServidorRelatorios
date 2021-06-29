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

        public async Task<IEnumerable<ConselhoClasseConsolidadoTurmaAlunoDto>> ObterConselhosClasseConsolidadoPorTurmasBimestreAsync(string[] turmasCodigo, int[] bimestres, int? situacaoConselhoClasse)
        {
            var query = new StringBuilder(@" select c.id, c.dt_atualizacao DataAtualizacao, c.status, c.aluno_codigo AlunoCodigo, 
                                                    c.parecer_conclusivo_id ParecerConclusivoId, c.turma_id TurmaId, c.bimestre
                            from consolidado_conselho_classe_aluno_turma c
                            inner join turma t on c.turma_id = t.id
                          where not c.excluido 
                            and t.turma_id = ANY(@turmasCodigo) ");

            if (bimestres != null && bimestres.Any())
                query.AppendLine(@"and c.bimestre = ANY(@bimestres)");

            if (situacaoConselhoClasse.HasValue)
                query.AppendLine(@"and EXISTS(select 1 from consolidado_conselho_classe_aluno_turma c2
                                               inner join turma t2 on c2.turma_id = t2.id
                                              where not c2.excluido and t2.turma_id = ANY(@turmasCodigo)
                                                and c2.bimestre = ANY(@bimestres) and c2.status = @situacaoConselhoClasse)");

            var parametros = new { turmasCodigo, bimestres, situacaoConselhoClasse };

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp);
            return await conexao.QueryAsync<ConselhoClasseConsolidadoTurmaAlunoDto>(query.ToString(), parametros);
        }
    }
}
