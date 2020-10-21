using Dapper;
using Npgsql;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class AulaRepository : IAulaRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public AulaRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<int> ObterAulasDadas(string codigoTurma, string componenteCurricularCodigo, long tipoCalendarioId, int bimestre)
        {
            var query = @"select sum(a.quantidade) 
                          from aula a 
                         inner join periodo_escolar p on p.tipo_calendario_id = a.tipo_calendario_id
 						                    and a.data_aula between p.periodo_inicio and p.periodo_fim
                         inner join registro_frequencia rf on rf.aula_id = a.id
                          where a.tipo_calendario_id = @tipoCalendarioId
                            and a.turma_id = @codigoTurma 
                            and a.disciplina_id = @disciplinaId 
                            and p.bimestre = @bimestre ";

            var parametros = new { 
                CodigoTurma = codigoTurma, 
                DisciplinaId = componenteCurricularCodigo,
                TipoCalendarioId = tipoCalendarioId,
                Bimestre = bimestre
            };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return await conexao.QueryFirstOrDefaultAsync<int?>(query, parametros) ?? 0;
            }
        }

        

        public async Task<bool> VerificaExisteAulaCadastrada(string turmaCodigo, string componenteCurricularId)
        {
            var query = @"select distinct 1 from aula where turma_id = @turmaCodigo and disciplina_id = @componenteCurricularId;";

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return await conexao.QueryFirstOrDefaultAsync<bool>(query, new { turmaCodigo, componenteCurricularId });
            }
        }
    }
}
