using Dapper;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class ProfessorRepository : IProfessorRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public ProfessorRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<IEnumerable<ProfessorTitularComponenteCurricularDto>> BuscarProfessorTitularComponenteCurricularPorCodigosRf(string[] codigosRf)
        {
            StringBuilder query = new StringBuilder();

            query.AppendLine(@"
				select coalesce(cc_pro.cd_componente_curricular, cc_ser.cd_componente_curricular) as ComponenteCurricularId,
					   coalesce(cc_pro.dc_componente_curricular, cc_ser.dc_componente_curricular) as ComponenteCurricular,
					   coalesce(serv.cd_registro_funcional, '')                                   as ProfessorRF,
					   coalesce(serv.nm_pessoa, N'Não há professor titular.')                     as NomeProfessor
                from turma_escola tur
                         -- Turma regular
                         left join serie_turma_grade ste on tur.cd_turma_escola = ste.cd_turma_escola and ste.dt_fim is null
                         left join escola_grade eg_ser
                                   on ste.cd_escola = eg_ser.cd_escola and ste.cd_escola_grade = eg_ser.cd_escola_grade
                         left join grade g_ser on eg_ser.cd_grade = g_ser.cd_grade
                         left join grade_componente_curricular gcc_ser on g_ser.cd_grade = gcc_ser.cd_grade
                         left join componente_curricular cc_ser on gcc_ser.cd_componente_curricular = cc_ser.cd_componente_curricular

                    -- Turma Programa
                         left join turma_escola_grade_programa tegp
                                   on tegp.cd_turma_escola = tur.cd_turma_escola and tegp.dt_fim is null
                         left join escola_grade eg_pro on eg_pro.cd_escola_grade = tegp.cd_escola_grade
                         left join grade g_pro on eg_pro.cd_grade = g_pro.cd_grade
                         left join grade_componente_curricular gcc_pro on g_pro.cd_grade = gcc_pro.cd_grade
                         left join componente_curricular cc_pro on gcc_pro.cd_componente_curricular = cc_pro.cd_componente_curricular

                    -- Atribuicao turma regular
                         left join atribuicao_aula atb_ser
                                   on gcc_ser.cd_grade = atb_ser.cd_grade and
                                      gcc_ser.cd_componente_curricular = atb_ser.cd_componente_curricular
                                       and atb_ser.cd_serie_grade = ste.cd_serie_grade and atb_ser.dt_cancelamento is null
                                       and (atb_ser.dt_disponibilizacao_aulas is null)
                                       and atb_ser.an_atribuicao = year(getdate())

                    -- Atribuicao turma programa
                         left join atribuicao_aula atb_pro
                                   on gcc_pro.cd_grade = atb_pro.cd_grade and
                                      gcc_pro.cd_componente_curricular = atb_pro.cd_componente_curricular and
                                      atb_pro.cd_turma_escola_grade_programa = tegp.cd_turma_escola_grade_programa and
                                      atb_pro.dt_cancelamento is null
                                       and atb_pro.dt_disponibilizacao_aulas is null
                                       and atb_pro.an_atribuicao = year(getdate())

                    -- Servidor
                         left join v_cargo_base_cotic vcbc on (atb_ser.cd_cargo_base_servidor = vcbc.cd_cargo_base_servidor or
                                                               atb_pro.cd_cargo_base_servidor = vcbc.cd_cargo_base_servidor)
                    and vcbc.dt_cancelamento is null and vcbc.dt_fim_nomeacao is null
                         left join v_servidor_cotic serv on serv.cd_servidor = vcbc.cd_servidor
                    WHERE serv.cd_registro_funcional in @codigosRf");

            var parametros = new { codigosRf };

            using (var conn = new SqlConnection(variaveisAmbiente.ConnectionStringEol))
            {
                var result = await conn.QueryAsync<ProfessorTitularComponenteCurricularDto>(
                                              query.ToString(),
                                              parametros, commandTimeout: 60000);
                return result.ToList();
            }
        }

        public async Task<IEnumerable<ProfessorTitularComponenteCurricularDto>> BuscarProfessorTitularComponenteCurricularPorTurma(string[] codigosTurma)
        {
            StringBuilder query = new StringBuilder();

            query.AppendLine(@"
				select coalesce(cc_pro.cd_componente_curricular, cc_ser.cd_componente_curricular) as ComponenteCurricularId,
					   coalesce(cc_pro.dc_componente_curricular, cc_ser.dc_componente_curricular) as ComponenteCurricular,
					   coalesce(serv.cd_registro_funcional, '')                                   as ProfessorRF,
					   coalesce(serv.nm_pessoa, N'Não há professor titular.')                     as NomeProfessor,
                       tur.cd_turma_escola TurmaCodigo
                from turma_escola tur
                         -- Turma regular
                         left join serie_turma_grade ste on tur.cd_turma_escola = ste.cd_turma_escola and ste.dt_fim is null
                         left join escola_grade eg_ser
                                   on ste.cd_escola = eg_ser.cd_escola and ste.cd_escola_grade = eg_ser.cd_escola_grade
                         left join grade g_ser on eg_ser.cd_grade = g_ser.cd_grade
                         left join grade_componente_curricular gcc_ser on g_ser.cd_grade = gcc_ser.cd_grade
                         left join componente_curricular cc_ser on gcc_ser.cd_componente_curricular = cc_ser.cd_componente_curricular

                    -- Turma Programa
                         left join turma_escola_grade_programa tegp
                                   on tegp.cd_turma_escola = tur.cd_turma_escola and tegp.dt_fim is null
                         left join escola_grade eg_pro on eg_pro.cd_escola_grade = tegp.cd_escola_grade
                         left join grade g_pro on eg_pro.cd_grade = g_pro.cd_grade
                         left join grade_componente_curricular gcc_pro on g_pro.cd_grade = gcc_pro.cd_grade
                         left join componente_curricular cc_pro on gcc_pro.cd_componente_curricular = cc_pro.cd_componente_curricular

                    -- Atribuicao turma regular
                         left join atribuicao_aula atb_ser
                                   on gcc_ser.cd_grade = atb_ser.cd_grade and
                                      gcc_ser.cd_componente_curricular = atb_ser.cd_componente_curricular
                                       and atb_ser.cd_serie_grade = ste.cd_serie_grade and atb_ser.dt_cancelamento is null
                                       and (atb_ser.dt_disponibilizacao_aulas is null)
                                       and atb_ser.an_atribuicao = year(getdate())

                    -- Atribuicao turma programa
                         left join atribuicao_aula atb_pro
                                   on gcc_pro.cd_grade = atb_pro.cd_grade and
                                      gcc_pro.cd_componente_curricular = atb_pro.cd_componente_curricular and
                                      atb_pro.cd_turma_escola_grade_programa = tegp.cd_turma_escola_grade_programa and
                                      atb_pro.dt_cancelamento is null
                                       and atb_pro.dt_disponibilizacao_aulas is null
                                       and atb_pro.an_atribuicao = year(getdate())

                    -- Servidor
                         left join v_cargo_base_cotic vcbc on (atb_ser.cd_cargo_base_servidor = vcbc.cd_cargo_base_servidor or
                                                               atb_pro.cd_cargo_base_servidor = vcbc.cd_cargo_base_servidor)
                    and vcbc.dt_cancelamento is null and vcbc.dt_fim_nomeacao is null
                         left join v_servidor_cotic serv on serv.cd_servidor = vcbc.cd_servidor
                    WHERE tur.cd_turma_escola in @codigosTurma");

            var parametros = new { codigosTurma };

            using (var conn = new SqlConnection(variaveisAmbiente.ConnectionStringEol))
            {
                var result = await conn.QueryAsync<ProfessorTitularComponenteCurricularDto>(
                                              query.ToString(),
                                              parametros);
                return result.ToList();
            }
        }

        public async Task<IEnumerable<ProfessorTitularComponenteCurricularDto>> BuscarProfessorTitularExternoComponenteCurricularPorTurma(string[] codigosTurma)
        {
            StringBuilder query = new StringBuilder();

            query.AppendLine(@"
				select coalesce(cc_pro.cd_componente_curricular, cc_ser.cd_componente_curricular) as ComponenteCurricularId,
					   coalesce(cc_pro.dc_componente_curricular, cc_ser.dc_componente_curricular) as ComponenteCurricular,
					   coalesce(p.cd_cpf_pessoa, '')                                   as ProfessorRF,
					   coalesce(p.nm_pessoa, N'Não há professor titular.')                     as NomeProfessor,
                       tur.cd_turma_escola TurmaCodigo
                from turma_escola tur
                         -- Turma regular
                         left join serie_turma_grade ste on tur.cd_turma_escola = ste.cd_turma_escola and ste.dt_fim is null
                         left join escola_grade eg_ser
                                   on ste.cd_escola = eg_ser.cd_escola and ste.cd_escola_grade = eg_ser.cd_escola_grade
                         left join grade g_ser on eg_ser.cd_grade = g_ser.cd_grade
                         left join grade_componente_curricular gcc_ser on g_ser.cd_grade = gcc_ser.cd_grade
                         left join componente_curricular cc_ser on gcc_ser.cd_componente_curricular = cc_ser.cd_componente_curricular

                    -- Turma Programa
                         left join turma_escola_grade_programa tegp
                                   on tegp.cd_turma_escola = tur.cd_turma_escola and tegp.dt_fim is null
                         left join escola_grade eg_pro on eg_pro.cd_escola_grade = tegp.cd_escola_grade
                         left join grade g_pro on eg_pro.cd_grade = g_pro.cd_grade
                         left join grade_componente_curricular gcc_pro on g_pro.cd_grade = gcc_pro.cd_grade
                         left join componente_curricular cc_pro on gcc_pro.cd_componente_curricular = cc_pro.cd_componente_curricular

                    -- Atribuicao turma regular
                         left join atribuicao_externo atb_ext
                                   on gcc_ser.cd_grade = atb_ext.cd_grade and
                                      gcc_ser.cd_componente_curricular = atb_ext.cd_componente_curricular
                                       and atb_ext.cd_serie_grade = ste.cd_serie_grade and atb_ext.dt_cancelamento is null
                                       and (atb_ext.dt_disponibilizacao is null)
                                       and atb_ext.an_atribuicao = year(getdate())

                    -- Atribuicao turma programa
                         left join atribuicao_externo atb_ext_prog
                                   on gcc_pro.cd_grade = atb_ext_prog.cd_grade and
                                      gcc_pro.cd_componente_curricular = atb_ext_prog.cd_componente_curricular and
                                      atb_ext_prog.cd_turma_escola_grade_programa = tegp.cd_turma_escola_grade_programa and
                                      atb_ext_prog.dt_cancelamento is null
                                       and atb_ext_prog.dt_disponibilizacao is null
                                       and atb_ext_prog.an_atribuicao = year(getdate())

                    inner join contrato_externo ce with (NOLOCK) on ce.cd_motivo_desligamento_externo is null and (ce.cd_contrato_externo = atb_ext.cd_contrato_externo or ce.cd_contrato_externo = atb_ext_prog.cd_contrato_externo)
                    -- Servidor                   	
                    inner join pessoa p with (NOLOCK) on p.cd_pessoa = ce.cd_pessoa 
                    WHERE tur.cd_turma_escola in @codigosTurma");

            var parametros = new { codigosTurma };

            using (var conn = new SqlConnection(variaveisAmbiente.ConnectionStringEol))
            {
                var result = await conn.QueryAsync<ProfessorTitularComponenteCurricularDto>(
                                              query.ToString(),
                                              parametros);
                return result.ToList();
            }
        }
    }
}
