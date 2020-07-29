using Dapper;
using Npgsql;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class ParecerConclusivoRepository : IParecerConclusivoRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public ParecerConclusivoRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<IEnumerable<RelatorioParecerConclusivoRetornoDto>> ObterPareceresFinais(int anoLetivo, string dreCodigo, string ueCodigo, Modalidade? modalidade, int? semestre,
                                                                                                            long cicloId, string[] anos, long parecerConclusivoId)
        {
            try
            {

                var query = new StringBuilder(@"select t.turma_id as TurmaId, 
	                                               cca.aluno_codigo AlunoCodigo, 
	                                               ccp.nome ParecerConclusivo, 
	                                               d.abreviacao as DreNome, 
	                                               te.descricao || ' - ' || u.nome as UeNome,
	                                               t.ano,
	                                               tc.descricao as Ciclo
	                                            from conselho_classe_aluno cca 
		                                            inner join conselho_classe_parecer ccp
			                                            on cca.conselho_classe_parecer_id = ccp.id 
		                                            inner join conselho_classe cc 
			                                            on cca.conselho_classe_id = cc.id
		                                            inner join fechamento_turma ft
			                                            on cc.fechamento_turma_id = ft.id
		                                            inner join turma t
			                                            on ft.turma_id  = t.id
		                                            inner join periodo_escolar pe
			                                            on ft.periodo_escolar_id = pe.id
		                                            inner join ue u 
			                                            on t.ue_id = u.id 
		                                            inner join dre d 
			                                            on u.dre_id  = d.id
		                                            inner join tipo_ciclo_ano tca
			                                            on tca.ano  = t.ano 
			                                            and tca.modalidade = t.modalidade_codigo 
		                                            inner join tipo_ciclo tc
			                                            on tca.tipo_ciclo_id = tc.id 
    	                                            inner join tipo_escola te
                                                        on te.id = u.tipo_escola where 1=1");

                if (semestre.HasValue)
                    query.AppendLine(" and t.semestre = @semestre ");

                if (cicloId > 0)
                    query.AppendLine(" and tc.id = @cicloId ");

                if (anos.Length > 0)
                    query.AppendLine(" and t.ano = ANY(@anos) ");

                if (parecerConclusivoId > 0)
                    query.AppendLine(" and ccp.id = @parecerConclusivoId ");

                if (modalidade.HasValue)
                    query.AppendLine(" and t.modalidade_codigo = @modalidadeId ");

                query.AppendLine("order by d.id, u.id, t.id");

                var parametros = new
                {
                    anoLetivo,
                    dreCodigo,
                    ueCodigo,
                    modalidadeId = modalidade.HasValue? (int)modalidade : 0,
                    semestre = semestre ?? 0,
                    cicloId,
                    parecerConclusivoId,
                    anos =  anos.ToString()                    
                };

                using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp);

                return await conexao.QueryAsync<RelatorioParecerConclusivoRetornoDto>(query.ToString(), parametros);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
