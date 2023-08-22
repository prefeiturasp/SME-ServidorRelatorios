using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nest;
using Newtonsoft.Json;
using SME.SR.Data.Interfaces.ElasticSearch.Base;
using SME.SR.Infra;

namespace SME.SR.Data.Repositories.ElasticSearch.Base
{
    public abstract class RepositorioElasticBase<T> : IRepositorioElasticBase<T> where T : class
    {
        private const int QUANTIDADE_RETORNO = 200;
        private const string TEMPO_CURSOR = "10s";
        private const string NOME_TELEMETRIA = "Elastic";
        private readonly IElasticClient _elasticClient;
        private readonly IServicoTelemetria servicoTelemetria;

        protected RepositorioElasticBase(IElasticClient elasticClient, IServicoTelemetria servicoTelemetria)
        {
            _elasticClient = elasticClient;
            this.servicoTelemetria = servicoTelemetria;
        }

        public async Task<IEnumerable<H>> ObterListaAsync<H>(string indice, Func<QueryContainerDescriptor<H>, QueryContainer> request, string nomeConsulta, object parametro = null)
            where H : class
        {
            var listaDeRetorno = new List<H>();

            ISearchResponse<H> response = await servicoTelemetria.RegistrarComRetornoAsync<ISearchResponse<H>>(async () =>
                                                                                       await _elasticClient.SearchAsync<H>(s => s.Index(indice)
                                                                                                                                .Query(request)
                                                                                                                                .Scroll(TEMPO_CURSOR)
                                                                                                                                .Size(QUANTIDADE_RETORNO)),
                                                                                       NOME_TELEMETRIA,
                                                                                       nomeConsulta, 
                                                                                       indice,
                                                                                       parametro?.ToString());

            if (!response.IsValid)
                throw new Exception(response.ServerError?.ToString(), response.OriginalException);

            listaDeRetorno.AddRange(response.Documents);

            while (response.Documents.Any() && response.Documents.Count == QUANTIDADE_RETORNO)
            {
                response = await servicoTelemetria.RegistrarComRetornoAsync<ISearchResponse<H>>(async () => 
                                                                                        await _elasticClient.ScrollAsync<H>(TEMPO_CURSOR, response.ScrollId),
                                                                                        NOME_TELEMETRIA,
                                                                                        nomeConsulta + " scroll",
                                                                                        indice,
                                                                                        parametro?.ToString());
                listaDeRetorno.AddRange(response.Documents);
            }

            this._elasticClient.ClearScroll(new ClearScrollRequest(response.ScrollId));

            return listaDeRetorno;
        }
    }
}