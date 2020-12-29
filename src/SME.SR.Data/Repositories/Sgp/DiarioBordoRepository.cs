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
    public class DiarioBordoRepository : IDiarioBordoRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public DiarioBordoRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<IEnumerable<AulaDiarioBordoDto>> ObterAulasDiarioBordo(long anoLetivo, int bimestre, string codigoUe, long componenteCurricular, bool listarDataFutura, string codigoTurma, Modalidade modalidadeTurma, ModalidadeTipoCalendario modalidadeCalendario, int semestre)
        {
            var query = $@"select a.data_aula as DataAula
	                        , t.nome as Turma
	                        , cc.descricao_sgp as ComponenteCurricular
	                        , pe.bimestre
	                        , db.criado_em as DataPlanejamento
	                        , db.criado_rf as UsuarioRf
	                        , db.criado_por as Usuario
	                        , db.planejamento
	                        , db.reflexoes_replanejamento as Reflexoes
	                        , db.devolutiva_id as DevolutivaId
                        from aula a 
                        inner join turma t on t.turma_id = a.turma_id 
                        inner join ue on ue.id = t.ue_id 
                        inner join componente_curricular cc on cc.Id = a.disciplina_id::bigint
                         left join diario_bordo db on db.aula_id  = a.id
                         left join tipo_calendario tc on tc.ano_letivo = @anoLetivo and tc.modalidade = @modalidadeCalendario and not tc.excluido
                         left join periodo_escolar pe on pe.tipo_calendario_id = tc.id and a.data_aula between pe.periodo_inicio and pe.periodo_fim 
                        where t.ano_letivo = @anoLetivo
                          and t.modalidade_codigo = @modalidadeTurma
                          and ue.ue_id = @codigoUe";

            if (bimestre != -99)
                query += " and pe.bimestre = @bimestre ";

            if (componenteCurricular != -99)
                query += " and cc.id = @componenteCurricular ";

            if (!listarDataFutura)
                query += " and a.data_aula <= NOW()::DATE ";

            if (codigoTurma != "-99")
                query += " and a.turma_id = @codigoTurma ";

            if (semestre > 0)
                query += " and t.semestre = @semestre ";
            query += "limit 200";

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return await conexao.QueryAsync<AulaDiarioBordoDto>(query, new { anoLetivo, bimestre, codigoUe, componenteCurricular, codigoTurma, modalidadeTurma, modalidadeCalendario, semestre });
            }
        }

        public async Task<DateTime?> ObterUltimoDiarioBordoProfessor(string professorRf)
        {
            var query = @"select max(d.criado_em)
                          from aula a 
                          inner join diario_bordo d on d.aula_id = a.id
                         where not a.excluido
                           and a.professor_rf = @professorRf";

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return await conexao.QueryFirstOrDefaultAsync<DateTime?>(query, new { professorRf });
            }
        }
    }
}
