using Dapper;
using Npgsql;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class ConselhoClasseAlunoTurmaComplementarRepository : IConselhoClasseAlunoTurmaComplementarRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public ConselhoClasseAlunoTurmaComplementarRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<IEnumerable<TurmaComplementarConselhoClasseAluno>> ObterTurmasPorConselhoClasseAlunoIds(long[] conselhoClasseAlunoIds)
        {
            var query = @"select cca.id as ConselhoClasseAlunoId
                            , ccatc.turma_id as TurmaComplementarId
                            , tc.turma_id as TurmaComplementarCodigo
	                        , ft.turma_id as TurmaRegularId
                            , tf.turma_id as TurmaRegularCodigo
                          from fechamento_turma ft
                         inner join turma tf on ft.turma_id = tf.id
                         inner join conselho_classe cc on cc.fechamento_turma_id = ft.id
                         inner join conselho_classe_aluno cca on cca.conselho_classe_id = cc.id
                         inner join conselho_classe_aluno_turma_complementar ccatc on ccatc.conselho_classe_aluno_id = cca.id
                         inner join turma tc on ccatc.turma_id = tc.id
                        where cca.id = ANY(@conselhoClasseAlunoIds)";

            var parametros = new { conselhoClasseAlunoIds };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryAsync<TurmaComplementarConselhoClasseAluno>(query, parametros);
            }
        }
    }
}
