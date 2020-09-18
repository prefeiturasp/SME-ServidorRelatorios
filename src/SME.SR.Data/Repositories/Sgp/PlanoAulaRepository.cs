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
                            plano_aula.id = objetivo_aprendizagem_aula.plano_aula_id
                        inner join objetivo_aprendizagem_plano on
                            objetivo_aprendizagem_aula.objetivo_aprendizagem_plano_id = objetivo_aprendizagem_plano.id
                        inner join objetivo_aprendizagem on
                            objetivo_aprendizagem_plano.objetivo_aprendizagem_jurema_id = objetivo_aprendizagem.id
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
	                        desenvolvimento_aula as DesenvolvimentoAula,
	                        recuperacao_aula as Recuperacao,
	                        licao_casa as LicaoCasa,
	                        ue.nome as Ue,
	                        dre.nome  as Dre,
	                        turma.nome  as Turma,
                            turma.turma_id  as TurmaCodigo,
                            plano_aula.criado_em as DataPlanoAula,
	                        componente_curricular.descricao_eol as ComponenteCurricular
                        from
	                        plano_aula
                        inner join aula on
	                        plano_aula.aula_id = aula.id
                        inner join ue on
	                        aula.ue_id = ue.ue_id
                        inner join dre on
	                        ue.dre_id = dre.id
                        inner join turma on
	                        aula.turma_id = turma.turma_id 
                        inner join componente_curricular on
	                        aula.disciplina_id::int8 = componente_curricular.codigo_eol 
                                                where plano_aula.id = @id
                        group by

                            plano_aula.id,
	                        desenvolvimento_aula,
	                        recuperacao_aula,
	                        licao_casa,
	                        ue.nome,
	                        dre.nome,
	                        turma.nome,
	                        componente_curricular.descricao_eol, aula.disciplina_id, turma.turma_id";

            var parametros = new
            {
                id = planoAulaId
            };

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp);
            return await conexao.QueryFirstOrDefaultAsync<PlanoAulaDto>(query, parametros);
        }
    }
}