using Dapper;
using Npgsql;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Data.Interfaces
{
    public class ItineranciaRepository : IItineranciaRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public ItineranciaRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<IEnumerable<ItineranciaAlunoDto>> ObterAlunosPorItineranciaIds(long[] ids)
        {
            var query = @" select ia.Id, ia.itinerancia_id as ItineranciaId, ia.codigo_aluno as AlunoCodigo
 	                        , iaq.id, q.ordem, q.nome, iaq.resposta
                          from itinerancia_aluno ia
                         inner join itinerancia_aluno_questao iaq on iaq.itinerancia_aluno_id = ia.id
                         inner join questao q on q.id = iaq.questao_id 
                         where not ia.excluido 
                           and ia.itinerancia_id = ANY(@ids)";

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                var lookup = new Dictionary<long, ItineranciaAlunoDto>();

                await conexao.QueryAsync<ItineranciaAlunoDto, ItineranciaQuestaoDto, ItineranciaAlunoDto>(query.ToString(),
                    (aluno, questao) =>
                    {
                        var retorno = new ItineranciaAlunoDto();
                        if (!lookup.TryGetValue(aluno.Id, out retorno))
                        {
                            retorno = aluno;
                            lookup.Add(aluno.Id, retorno);
                        }

                        retorno.Questoes.Add(questao);

                        return retorno;
                    },
                    new { ids });

                return lookup.Values;
            }
        }

        public async Task<IEnumerable<Itinerancia>> ObterComUEDREPorIds(long[] ids)
        {
            var query = @"select i.id, i.data_visita as DataVisita, i.data_retorno_verificacao as DataRetorno, i.situacao, i.ano_letivo as AnoLetivo
	                        , ue.id, ue.ue_id as Codigo, ue.nome, ue.tipo_escola as TipoEscola
	                        , dre.id, dre.dre_id as Codigo, dre.abreviacao, dre.nome 
                          from itinerancia i 
                         inner join ue on ue.id = i.ue_id 
                         inner join dre on dre.id = ue.dre_id
                         where i.id = ANY(@ids)
                        ";

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryAsync<Itinerancia, Ue, Dre, Itinerancia>(query.ToString(),
                    (itinerancia, ue, dre) =>
                    {
                        ue.Dre = dre;
                        itinerancia.Ue = ue;

                        return itinerancia;
                    },
                    new { ids });
            }
        }

        public async Task<IEnumerable<ItineranciaObjetivoDto>> ObterObjetivosPorItineranciaIds(long[] ids)
        {
            var query = @"select io.itinerancia_id as ItineranciaId, iob.ordem, iob.nome, io.descricao 
                           from itinerancia_objetivo io 
                          inner join itinerancia_objetivo_base iob on iob.id = io.itinerancia_base_id 
                          where not io.excluido 
                            and io.itinerancia_id = ANY(@ids)";

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryAsync<ItineranciaObjetivoDto>(query.ToString(), new { ids });
            }
        }

        public async Task<IEnumerable<ItineranciaQuestaoDto>> ObterQuestoesPorItineranciaIds(long[] ids)
        {
            var query = @"select iq.itinerancia_id as ItineranciaId, q.ordem, q.nome, iq.resposta,q.tipo as TipoQuestao
                          from itinerancia_questao iq 
                         inner join questao q on q.id = iq.questao_id 
                         where not iq.excluido 
                           and iq.itinerancia_id = ANY(@ids)";

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryAsync<ItineranciaQuestaoDto>(query.ToString(), new { ids });
            }
        }
    }
}
