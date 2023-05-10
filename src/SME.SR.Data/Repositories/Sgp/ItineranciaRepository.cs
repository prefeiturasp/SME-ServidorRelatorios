using Dapper;
using Npgsql;
using Org.BouncyCastle.Crypto;
using SME.SR.Infra;
using SME.SR.Infra.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
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

        private string ObterCondicaoDre(FiltroRelatorioListagemItineranciasDto filtro) =>
                    !filtro.DreCodigo.EstaFiltrandoTodas() ? " and d.dre_id = @dreCodigo " : string.Empty;

        private string ObterCondicaoUe(FiltroRelatorioListagemItineranciasDto filtro) =>
                    !filtro.UeCodigo.EstaFiltrandoTodas() ? " and u.ue_id = @ueCodigo " : string.Empty;

        private string ObterCodicaoSituacao(FiltroRelatorioListagemItineranciasDto filtro)
        {
            var condicao = string.Empty;

            if (!filtro.SituacaoIds.EstaFiltrandoTodas())
                condicao += " and i.situacao = ANY(@situacaoIds) ";

            return condicao;
        }

        private string ObterAno(FiltroRelatorioListagemItineranciasDto filtro)
        {
            var condicao = string.Empty;

            if (filtro.AnoLetivo > 0)
                condicao += " and i.ano_letivo = @anoLetivo ";

            return condicao;
        }

        private string ObterCodicaoPAAI(FiltroRelatorioListagemItineranciasDto filtro) =>
                   !filtro.CodigosPAAIResponsavel.EstaFiltrandoTodas() ? " and i.criado_rf = ANY(@codigosPAAIResponsavel) " : string.Empty;

        private string ObterCondicao(FiltroRelatorioListagemItineranciasDto filtro)
        {
            var query = new StringBuilder();
            var funcoes = new List<Func<FiltroRelatorioListagemItineranciasDto, string>>
            {
                ObterCondicaoDre,
                ObterCondicaoUe,
                ObterCodicaoSituacao,
                ObterCodicaoPAAI,
                ObterAno
            };

            foreach (var funcao in funcoes)
            {
                query.Append(funcao(filtro));
            }

            return query.ToString();
        }

        public async Task<IEnumerable<ListagemItineranciaDto>> ObterItinerancias(FiltroRelatorioListagemItineranciasDto filtro)
        {
            var query = @"select  i.id, 
		                    i.data_visita as DataVisita, 
		                    i.situacao, 
		                    i.ano_letivo as AnoLetivo,
		                    i.criado_por as ResponsavelPaaiNome,
		                    i.criado_rf as ResponsavelPaaiLoginRf,
	                        u.ue_id as uecodigo,
		                    u.nome as uenome,
		                    u.tipo_escola as tipoescola,
		                    d.dre_id as drecodigo,
                            d.nome as drenome,
		                    d.abreviacao as dreabreviacao,
		                    io.id as objetivo_id, iob.nome, io.descricao, 
		                    ia.id as itinerancia_aluno_id,
		                    ia.codigo_aluno as codigo
                      from itinerancia i 
                     inner join ue u on u.id = i.ue_id 
                     inner join dre d on d.id = u.dre_id
                     inner join itinerancia_objetivo io on io.itinerancia_id = i.id and not io.excluido
                     inner join itinerancia_objetivo_base iob on iob.id = io.itinerancia_base_id 
                     left join usuario responsavel on responsavel.rf_codigo = i.criado_rf 
                     left join itinerancia_aluno ia on ia.itinerancia_id  = i.id and not ia.excluido
                     where not i.excluido ";

            query += ObterCondicao(filtro);
            
            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                var lookup = new Dictionary<long, ListagemItineranciaDto>();

                await conexao.QueryAsync<ListagemItineranciaDto, ObjetivoItineranciaDto, AlunoItineranciaDto,  ListagemItineranciaDto>(query.ToString(),
                    (itinerancia, objetivo, aluno) =>
                    {
                        var retorno = new ListagemItineranciaDto();
                        if (!lookup.TryGetValue(itinerancia.Id, out retorno))
                        {
                            retorno = itinerancia;
                            lookup.Add(itinerancia.Id, retorno);
                        }
                        if (!string.IsNullOrEmpty(objetivo.Nome) && !retorno.Objetivos.Any(x => x.Nome == objetivo.Nome))
                            retorno.Objetivos.Add(new ObjetivoItineranciaDto { Nome = objetivo.Nome, Descricao = objetivo.Descricao });
                        if (!string.IsNullOrEmpty(aluno.Codigo) && !retorno.Alunos.Any(x => x.Codigo == aluno.Codigo))
                            retorno.Alunos.Add(new AlunoItineranciaDto { Nome = null, Codigo = aluno.Codigo });

                        return retorno;
                    },
                    new {
                        dreCodigo = filtro.DreCodigo,
                        ueCodigo = filtro.UeCodigo,
                        situacaoIds = filtro.SituacaoIds,
                        codigosPAAIResponsavel = filtro.CodigosPAAIResponsavel,
                        anoLetivo = filtro.AnoLetivo
                    },
                    splitOn: "id,objetivo_id,itinerancia_aluno_id");

                return lookup.Values;
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
