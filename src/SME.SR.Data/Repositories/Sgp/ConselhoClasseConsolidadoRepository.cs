using Dapper;
using Npgsql;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
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

        public async Task<IEnumerable<ConselhoClasseConsolidadoTurmaAlunoDto>> ObterConselhosClasseConsolidadoPorTurmasBimestreAsync(long[] turmasId, int bimestre, int situacaoConselhoClasse)
        {
            var query = new StringBuilder(@" select id, dt_atualizacao DataAtualizacao, status, aluno_codigo AlunoCodigo, 
                                                    parecer_conclusivo_id ParecerConclusivoId, turma_id TurmaId, bimestre
                            from consolidado_conselho_classe_aluno_turma 
                          where not excluido 
                            and turma_id = ANY(@turmasId) ");

            if (bimestre != -99)
                query.AppendLine(@"and bimestre = @bimestre");

            if (situacaoConselhoClasse != -99)
                query.AppendLine(@"and EXISTS(select 1 from consolidado_conselho_classe_aluno_turma
                                              where not excluido and turma_id = @turmaId 
                                                and bimestre = @bimestre and status = @situacaoConselhoClasse)");

            var parametros = new { turmasId, bimestre, situacaoConselhoClasse };

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp);
            return await conexao.QueryAsync<ConselhoClasseConsolidadoTurmaAlunoDto>(query.ToString(), parametros);
        }
    }
}
