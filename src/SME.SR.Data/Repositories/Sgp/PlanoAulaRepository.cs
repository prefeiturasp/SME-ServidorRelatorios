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

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp);
            return await conexao.QueryAsync<ObjetivoAprendizagemDto>(query, parametros);
        }

        public async Task<PlanoAulaDto> ObterPorId(long planoAulaId)
        {
            var query = @"select
	                        plano_aula.id as Id,
                            plano_aula.descricao as Descricao,
	                        desenvolvimento_aula as DesenvolvimentoAula,
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
	                        desenvolvimento_aula,
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

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp);
            return await conexao.QueryFirstOrDefaultAsync<PlanoAulaDto>(query, parametros);
        }

        public async Task<DateTime> ObterUltimoPlanoAulaProfessor(string professorRf)
        {
            var query = @"select max(pa.criado_em)
                      from aula a 
                      inner join plano_aula pa on pa.aula_id = a.id
                     where not a.excluido
                       and a.professor_rf = @professorRf";

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return await conexao.QueryFirstOrDefaultAsync<DateTime>(query, new { professorRf });
            }
        }
    }
}