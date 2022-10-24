using Dapper;
using Npgsql;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class RegistroFrequenciaRepository : IRegistroFrequenciaRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public RegistroFrequenciaRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<IEnumerable<TurmaComponenteQtdAulasDto>> ObterTotalAulasPorDisciplinaETurmaEBimestre(string[] turmasCodigo, string[] componentesCurricularesId, long tipoCalendarioId, int[] bimestres)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine(@"select a.disciplina_id as ComponenteCurricularCodigo, a.turma_id as TurmaCodigo, p.bimestre as Bimestre, 
                COALESCE(SUM(a.quantidade),0) AS AulasQuantidade from 
            aula a 
            inner join registro_frequencia rf on 
            rf.aula_id = a.id 
            inner join periodo_escolar p on 
            a.tipo_calendario_id = p.tipo_calendario_id 
            where not a.excluido 
            and a.data_aula >= p.periodo_inicio 
            and a.data_aula <= p.periodo_fim ");

            if (tipoCalendarioId > 0)
                query.AppendLine(" and a.tipo_calendario_id = @tipoCalendarioId ");
            if (bimestres != null && bimestres.Length > 0)
                query.AppendLine(" and p.bimestre = any(@bimestres) ");
            if (componentesCurricularesId != null && componentesCurricularesId.Length > 0)
                query.AppendLine("and a.disciplina_id = any(@componentesCurricularesId) ");

            query.AppendLine("and a.turma_id = any(@turmasCodigo) group by a.disciplina_id, a.turma_id, p.bimestre");
            
            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryAsync<TurmaComponenteQtdAulasDto>(query.ToString(), new { turmasCodigo, componentesCurricularesId, tipoCalendarioId, bimestres });
            }
        }
    }
}
