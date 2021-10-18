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

        public async Task<IEnumerable<ConsolidacaoRegistrosPedagogicosDto>> ObterDadosConsolidacaoRegistrosPedagogicos(int anoLetivo, long[] componentesCurriculares, long[] turmasId, string professorCodigo, string professorNome, List<int> bimestres)
        {
            var query = new StringBuilder(@"select 
                                            crp.periodo_escolar_id as PeriodoEscolarId,
                                            pe.bimestre as Bimestre,
                                            crp.turma_id as TurmaId,
                                            t.nome as TurmaNome,
                                            t.modalidade_codigo as TurmaModalidade,
                                            crp.ano_letivo as AnoLetivo,
                                            crp.componente_curricular_id as ComponenteCurricularId,
                                            cc.descricao_sgp as ComponenteCurricularNome,
                                            crp.quantidade_aulas as QuantidadeAulas,
                                            crp.frequencias_pendentes as FrequenciasPendentes,
                                            crp.data_ultima_frequencia as DataUltimaFrequencia,
                                            crp.data_ultimo_diariobordo as DataUltimoDiarioBordo,
                                            crp.data_ultimo_planoaula as DataUltimoPlanoAula,
                                            crp.diario_bordo_pendentes as DiarioBordoPendentes,
                                            crp.planos_aula_pendentes as PlanoAulaPendentes,
                                            crp.nome_professor as NomeProfessor,
                                            crp.rf_professor as RFProfessor
                                            from consolidacao_registros_pedagogicos crp
                                            inner join turma t on t.id = crp.turma_id
                                            inner join periodo_escolar pe on pe.id = crp.periodo_escolar_id
                                            inner join componente_curricular cc on cc.id = crp.componente_curricular_id
                                            where crp.ano_letivo = @anoLetivo");

            if(componentesCurriculares.Length > 0)
            {
                query.AppendLine(@" and crp.componente_curricular_id in (@componentesCurriculares)");
            }
            if(turmasId.Length > 0)
            {
                query.AppendLine(@" and crp.turma_id in (@turmasId)");
            }
            if(professorCodigo != null && professorCodigo != "")
            {
                query.AppendLine(@" and crp.rf_professor in (@professorCodigo)");
            }
            if (professorNome != null && professorNome != "")
            {
                query.AppendLine(@" and crp.nome_professor in (@professorNome)");
            }
            if (bimestres.Any())
            {
                query.AppendLine(@" and pe.bimestre in (@bimestres)");
            }

            var parametros = new
            {
                anoLetivo,
                componentesCurriculares,
                turmasId,
                professorCodigo,
                professorNome,
                bimestres
            };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return await conexao.QueryAsync<ConsolidacaoRegistrosPedagogicosDto>(query.ToString(), parametros);
            }
        }
    }
}
