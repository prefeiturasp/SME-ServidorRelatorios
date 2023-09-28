using Dapper;
using Npgsql;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class PlanoAulaRepository : IPlanoAulaRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public PlanoAulaRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<IEnumerable<ObjetivoAprendizagemDto>> ObterObjetivoAprendizagemPorPlanoAulaId(long planoAulaId)
        {
            var query = @"select
	                        objetivo_aprendizagem.codigo as Codigo,
	                        objetivo_aprendizagem.descricao as Descricao
                        from
                            plano_aula
                        inner join objetivo_aprendizagem_aula on
                            objetivo_aprendizagem_aula.plano_aula_id = plano_aula.id
                        inner join objetivo_aprendizagem on
                            objetivo_aprendizagem.id = objetivo_aprendizagem_aula.objetivo_aprendizagem_id
                        where plano_aula.id = @id";

            var parametros = new
            {
                id = planoAulaId
            };

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas);
            return await conexao.QueryAsync<ObjetivoAprendizagemDto>(query, parametros);
        }

        public async Task<PlanoAulaDto> ObterPorId(long planoAulaId)
        {
            var query = @"select
	                        plano_aula.id as Id,
                            plano_aula.descricao as Descricao,
	                        recuperacao_aula as Recuperacao,
	                        licao_casa as LicaoCasa,
	                        ue.nome as Ue,
                            te.descricao as TipoEscola,
	                        dre.abreviacao  as Dre,
                            turma.modalidade_codigo  as ModalidadeTurma,
	                        turma.nome  as Turma,
                            turma.turma_id  as TurmaCodigo,
                            aula.data_aula as DataPlanoAula,
                            aula.disciplina_id::int8 as ComponenteCurricularId
                        from
	                        plano_aula
                        inner join aula on
	                        plano_aula.aula_id = aula.id
                        inner join ue on
	                        aula.ue_id = ue.ue_id
                        inner join tipo_escola te on ue.tipo_escola = te.id 
                        inner join dre on
	                        ue.dre_id = dre.id
                        inner join turma on
	                        aula.turma_id = turma.turma_id 
                        where plano_aula.id = @id
                        group by
                            plano_aula.id,
	                        recuperacao_aula,
                            te.descricao,
	                        licao_casa,
	                        ue.nome,
	                        dre.abreviacao,
                            turma.modalidade_codigo,
	                        turma.nome,
                            aula.disciplina_id,
                            aula.data_aula,
                            aula.disciplina_id, turma.turma_id";

            var parametros = new
            {
                id = planoAulaId
            };

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas);
            return await conexao.QueryFirstOrDefaultAsync<PlanoAulaDto>(query, parametros);
        }

        public async Task<DateTime?> ObterUltimoPlanoAulaProfessor(string professorRf)
        {
            var query = @"select max(pa.criado_em)
                      from aula a 
                      inner join plano_aula pa on pa.aula_id = a.id
                     where not a.excluido
                       and a.professor_rf = @professorRf";

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryFirstOrDefaultAsync<DateTime?>(query, new { professorRf });
            }
        }

        public async Task<IEnumerable<AulaPlanoAulaDto>> ObterPlanejamentoDiarioPlanoAula(long anoLetivo, int bimestre, string codigoUe, long componenteCurricular, bool listarDataFutura, string codigoTurma, Modalidade modalidadeTurma, ModalidadeTipoCalendario modalidadeCalendario, int semestre)
        {            
              var query = @"select distinct
                                   a.id as AulaId,
                                   a.aula_cj as AulaCJ,
                                   t.nome as Turma,
                                   cc.descricao_sgp as ComponenteCurricular,
                                   pe.bimestre,
	                               a.data_aula as DataAula, 
                                   a.quantidade as QuantidadeAula,
                                   pa.criado_em as DataPlanejamento,
                                   pa.criado_por as Usuario,
                                   pa.criado_rf as UsuarioRf,
                                   pa.descricao as ObjetivosEspecificos,
                                   pa.licao_casa as LicaoCasa,
                                   pa.recuperacao_aula as RecuperacaoContinua,
                                   string_agg(oa.codigo, '<br/>') as ObjetivosSalecionados,
                                   count(oa.codigo) as QtdObjetivosSelecionados 
                              from aula a
                             inner join turma t on t.turma_id = a.turma_id
                             inner join ue on ue.id = t.ue_id 
                             left join componente_curricular cc on cc.Id = a.disciplina_id::bigint
                              left join plano_aula pa on pa.aula_id = a.id
                              left join objetivo_aprendizagem_aula oaa on oaa.plano_aula_id = pa.id 
	                          left join objetivo_aprendizagem oa on oa.id = oaa.objetivo_aprendizagem_id 
                             left join tipo_calendario tc on tc.ano_letivo = @anoLetivo and tc.modalidade = @modalidadeCalendario and not tc.excluido
                             left join periodo_escolar pe on pe.tipo_calendario_id = tc.id and a.data_aula between pe.periodo_inicio and pe.periodo_fim 
                             where t.ano_letivo = @anoLetivo
                               and t.modalidade_codigo = @modalidadeTurma
                               and a.ue_id = @codigoUe
                               and not a.excluido";

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

            query += @"group by a.id, a.aula_cj, t.nome,
                        cc.descricao_sgp, pe.bimestre, a.data_aula,
                        a.quantidade, pa.criado_em, pa.criado_por,
                        pa.criado_rf, pa.descricao, pa.licao_casa,
                        pa.recuperacao_aula, oaa.plano_aula_id";

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryAsync<AulaPlanoAulaDto>(query, new { anoLetivo, bimestre, codigoUe, componenteCurricular, codigoTurma, modalidadeTurma, modalidadeCalendario, semestre });
            }
        }
    }
}