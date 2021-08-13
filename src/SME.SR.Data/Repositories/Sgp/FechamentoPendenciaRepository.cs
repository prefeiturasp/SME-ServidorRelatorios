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
    public class FechamentoPendenciaRepository : IFechamentoPendenciaRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public FechamentoPendenciaRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<IEnumerable<RelatorioFechamentoPendenciasQueryRetornoDto>> ObterPendencias(int anoLetivo, string dreCodigo, string ueCodigo, long modalidadeId, int? semestre,
                                                                                                    string[] turmasCodigo, long[] componentesCodigo, int bimestre,bool pendenciaResolvida)
        {
            try
            {


                var query = new StringBuilder(@"select 
	                        p.titulo,
	                        p.descricao as Detalhe,
	                        p.situacao,
	                        d.abreviacao as DreNome,
	                        te.descricao || ' - ' || u.nome as UeNome,
	                        t.ano_letivo as AnoLetivo,
	                        t.modalidade_codigo as ModalidadeCodigo,
	                        t.semestre,
	                        t.nome as TurmaNome,
                            t.turma_id as TurmaCodigo,
	                        ftd.disciplina_id as DisciplinaId,
	                        pe.bimestre,
	                        p.criado_por as criador,
	                        p.criado_rf as criadorRf,
	                        p.alterado_por as aprovador,
	                        p.alterado_rf as aprovadorRf
                        from pendencia_fechamento pf
                        inner join pendencia p 
	                        on pf.pendencia_id  = p.id
                        inner join fechamento_turma_disciplina ftd 
	                        on ftd.id  = pf.fechamento_turma_disciplina_id 
                        inner join fechamento_turma ft
	                        on ftd.fechamento_turma_id = ft.id 
                        inner join turma t 
	                        on t.id = ft.turma_id 	
                        inner join ue u 
	                        on t.ue_id  = u.id 
                        inner join dre d 
	                        on u.dre_id  = d.id 
                        inner join periodo_escolar pe 
	                        on ft.periodo_escolar_id  = pe.id 
                        inner join tipo_escola te
                            on te.id = u.tipo_escola
                        where t.ano_letivo = @anoLetivo
                        and d.dre_id  = @dreCodigo
                        and u.ue_id  = @ueCodigo
                        and t.modalidade_codigo = @modalidadeId  
                        and not p.excluido");
                if (pendenciaResolvida)
                    query.AppendLine(" and p.situacao =3 ");
                else
                    query.AppendLine(" and p.situacao in(1,2) ");

                if(semestre.HasValue)
                    query.AppendLine(" and t.semestre = @semestre ");

                if (turmasCodigo.Length > 0)
                    query.AppendLine(" and t.turma_id = any(@turmasCodigo) ");

                if (componentesCodigo.Length > 0)
                    query.AppendLine(" and ftd.disciplina_id = any(@componentesIds)");

                if (bimestre > 0)
                    query.AppendLine(" and pe.bimestre  = @bimestre");

                var parametros = new
                {
                    anoLetivo,
                    dreCodigo,
                    ueCodigo,
                    modalidadeId,
                    semestre = semestre ?? 0,
                    turmasCodigo = turmasCodigo.ToList(),
                    componentesIds = componentesCodigo.ToList(),
                    bimestre
                };

                using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp);

                return await conexao.QueryAsync<RelatorioFechamentoPendenciasQueryRetornoDto>(query.ToString(), parametros) ;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
