using Newtonsoft.Json;
using Refit;
using SME.SR.JRSClient.Extensions;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SME.SR.JRSClient.Services
{
    public abstract class ServiceBase<T>
    {
        protected readonly Configuracoes configuracoes;
        protected readonly T restService;

        protected RefitSettings settings
        {
            get
            {
                return new RefitSettings()
                {
                    AuthorizationHeaderValueGetter = () => Task.FromResult(ObterUsuarioSenhaBase64()),
                    ContentSerializer = new NewtonsoftJsonContentSerializer(new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                    }),
                };
            }
        }

        public ServiceBase(Configuracoes configuracoes)
        {
            this.configuracoes = configuracoes ?? throw new ArgumentNullException(nameof(configuracoes));
            this.restService = RestService.For<T>(configuracoes.UrlBase, settings);
        }

        public ServiceBase(HttpClient httpClient, Configuracoes configuracoes)
        {
            this.configuracoes = configuracoes ?? throw new ArgumentNullException(nameof(configuracoes));
            this.restService = RestService.For<T>(httpClient, settings);
        }

        public string ObterCabecalhoAutenticacaoBasica()
        {
            return "Basic " + ObterUsuarioSenhaBase64();
        }

        public string ObterUsuarioSenhaBase64()
        {
            return $"{configuracoes?.JasperLogin}:{configuracoes?.JasperPassword}".EncodeTo64();
        }
    }
}
