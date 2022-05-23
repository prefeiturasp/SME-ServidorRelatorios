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
    public class RegistrosPedagogicosRepository : IRegistrosPedagogicosRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public RegistrosPedagogicosRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<IEnumerable<ConsolidacaoRegistrosPedagogicosDto>> ObterDadosConsolidacaoRegistrosPedagogicos(string dreCodigo, string ueCodigo, int anoLetivo,
            long[] componentesCurriculares, List<string> turmasCodigo, string professorCodigo, List<int> bimestres, Modalidade modalidade, int semestre)
        {
            var turmaCodigo = turmasCodigo?.ToArray();
            int periodoCalendario = 0;

            var query = new StringBuilder(@"select distinct
                                            crp.periodo_escolar_id as PeriodoEscolarId,
                                            pe.bimestre as Bimestre,
                                            crp.turma_id as TurmaId,
                                            t.nome as TurmaNome,
                                            t.modalidade_codigo as ModalidadeCodigo,
                                            crp.ano_letivo as AnoLetivo,
                                            crp.componente_curricular_id as ComponenteCurricularId,
                                            cc.descricao_sgp as ComponenteCurricularNome,
                                            crp.quantidade_aulas as QuantidadeAulas,
                                            crp.frequencias_pendentes as FrequenciasPendentes,
                                            crp.data_ultima_frequencia as DataUltimaFrequencia,
                                            crp.data_ultimo_planoaula as DataUltimoPlanoAula,
                                            crp.planos_aula_pendentes as PlanoAulaPendentes,
                                            crp.nome_professor as NomeProfessor,
                                            crp.rf_professor as RFProfessor,
                                            crp.cj as CJ
                                            from consolidacao_registros_pedagogicos crp
                                            inner join turma t on t.id = crp.turma_id
                                            inner join periodo_escolar pe on pe.id = crp.periodo_escolar_id
                                            inner join tipo_calendario tc on tc.id = pe.tipo_calendario_id
                                            inner join componente_curricular cc on cc.id = crp.componente_curricular_id
                                            inner join ue ue on ue.id = t.ue_id
                                            inner join dre dre on dre.id = ue.dre_id
                                            where crp.ano_letivo = @anoLetivo");

            if (dreCodigo != null)
            {
                query.AppendLine(@" and dre.dre_id = @dreCodigo");
            }
            if (ueCodigo != null)
            {
                query.AppendLine(@" and ue.ue_id = @ueCodigo");
            }
            if (componentesCurriculares != null)
            {
                query.AppendLine(@" and crp.componente_curricular_id = any(@componentesCurriculares)");
            }
            if (turmaCodigo != null)
            {
                query.AppendLine(@" and t.turma_id = any(@turmaCodigo)");
            }
            if (professorCodigo != null && professorCodigo != "")
            {
                query.AppendLine(@" and crp.rf_professor in (@professorCodigo)");
            }
            if (bimestres != null && bimestres.Any())
            {
                query.AppendLine(@" and pe.bimestre = any(@bimestres)");
            }

            int codigoModalidade = (int)modalidade;
            query.AppendLine(@"and t.modalidade_codigo in (@codigoModalidade)");

            if (semestre > 0)
            {
                periodoCalendario = (int)Periodo.Semestral;
                query.AppendLine(@" and tc.periodo = @periodoCalendario and pe.bimestre = @semestre");
            }

            var parametros = new
            {
                dreCodigo,
                ueCodigo,
                anoLetivo,
                componentesCurriculares,
                turmaCodigo,
                professorCodigo,
                bimestres,
                codigoModalidade,
                periodoCalendario,
                semestre
            };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return await conexao.QueryAsync<ConsolidacaoRegistrosPedagogicosDto>(query.ToString(), parametros);
            }
        }

        public async Task<IEnumerable<ConsolidacaoRegistrosPedagogicosDto>> ObterDadosConsolidacaoRegistrosPedagogicosInfantil(string dreCodigo, string ueCodigo, int anoLetivo,
            string professorCodigo, List<int> bimestres, List<string> turmasCodigo = null, List<long> componentesCurricularesIds = null)
        {
            var query = new StringBuilder(@"select distinct
                                            crp.periodo_escolar_id as PeriodoEscolarId,
                                            pe.bimestre as Bimestre,
                                            crp.turma_id as TurmaId,
                                            t.nome as TurmaNome,
                                            t.modalidade_codigo as ModalidadeCodigo,
                                            crp.ano_letivo as AnoLetivo,
                                            crp.componente_curricular_id as ComponenteCurricularId,
                                            coalesce (cc.descricao_infantil, cc.descricao_sgp) as ComponenteCurricularNome,
                                            crp.quantidade_aulas as QuantidadeAulas,
                                            crp.frequencias_pendentes as FrequenciasPendentes,
                                            crp.data_ultima_frequencia as DataUltimaFrequencia,
                                            crp.data_ultimo_diariobordo as DataUltimoDiarioBordo,
                                            crp.diario_bordo_pendentes as DiarioBordoPendentes,
                                            crp.nome_professor as NomeProfessor,
                                            crp.rf_professor as RFProfessor,
                                            crp.cj as CJ
                                            from consolidacao_registros_pedagogicos crp
                                            inner join turma t on t.id = crp.turma_id
                                            inner join periodo_escolar pe on pe.id = crp.periodo_escolar_id
                                            inner join componente_curricular cc on cc.id = crp.componente_curricular_id
                                            inner join ue ue on ue.id = t.ue_id
                                            inner join dre dre on dre.id = ue.dre_id
                                            where crp.ano_letivo = @anoLetivo");

            if (dreCodigo != null)
            {
                query.AppendLine(@" and dre.dre_id = @dreCodigo");
            }

            if (ueCodigo != null)
            {
                query.AppendLine(@" and ue.ue_id = @ueCodigo");
            }

            if (turmasCodigo != null && turmasCodigo.Any())
            {
                query.AppendLine(@" and t.turma_id = Any(@turmasCodigo)");
            }

            if (professorCodigo != null && professorCodigo != "")
            {
                query.AppendLine(@" and crp.rf_professor in (@professorCodigo)");
            }

            if (bimestres != null && bimestres.Any())
            {
                query.AppendLine(@" and pe.bimestre = any(@bimestres)");
            }

            if (componentesCurricularesIds != null && componentesCurricularesIds.Any())
            {
                query.AppendLine(@" and crp.componente_curricular_id = Any(@componentesCurricularesIds)");
            }

            query.AppendLine(" order by t.nome ");

            var parametros = new
            {
                dreCodigo,
                ueCodigo,
                anoLetivo,
                turmasCodigo,
                professorCodigo,
                bimestres,
                componentesCurricularesIds
            };

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp);
            return await conexao.QueryAsync<ConsolidacaoRegistrosPedagogicosDto>(query.ToString(), parametros);
        }
    }
}
