using Dapper;
using Npgsql;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Data.Repositories.Sgp
{
    public class DevolutivaRepository : IDevolutivaRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public DevolutivaRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<IEnumerable<DevolutivaDto>> ObterDevolutivas(long ueId, IEnumerable<long> turmas, IEnumerable<int> bimestres)
        {
            var query = new StringBuilder(@"select d.id, d.periodo_inicio as DataInicio, d.periodo_fim as DataFim, d.criado_em as DataRegistro, d.criado_por as RegistradoPor, d.descricao 
	                        , a.id, a.data_aula as Data
	                        , t.id, t.nome
	                        , pe.id, pe.periodo_inicio as DataInicio, pe.periodo_fim as DataFim, pe.bimestre 
                          from devolutiva d 
                         inner join diario_bordo db on d.id = db.devolutiva_id 
                         inner join aula a on a.id = db.aula_id 
                         inner join turma t on t.turma_id = a.turma_id 
                         inner join tipo_calendario tc on tc.ano_letivo = t.ano_letivo 
 	                        and tc.modalidade = case t.modalidade_codigo 
 		                        when 1 then 3
 		                        when 3 then 2
 		                        else 1 end
                          left join periodo_escolar pe on pe.tipo_calendario_id = tc.id and a.data_aula between pe.periodo_inicio and pe.periodo_fim 
                        where t.ue_id = ueId ");

            if (turmas.Any())
                query.AppendLine(" and t.id = Any(@turmas)");

            if (bimestres.Any())
                query.AppendLine(" and pe.bimestre = Any(@bimestres)");

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return await conexao.QueryAsync<DevolutivaDto, DataAulaDto, TurmaNomeDto, PeriodoEscolarDto, DevolutivaDto>(query.ToString()
                    , (devolutiva, aula, turma, periodoEscolar) =>
                    {
                        aula.Turma = turma;
                        aula.PeriodoEscolar = periodoEscolar;
                        devolutiva.Aula = aula;

                        return devolutiva;
                    }
                    , new { ueId, turmas, bimestres });
            }

        }
    }
}
