using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SME.SR.JRSClient.Interfaces;
using SME.SR.JRSClient.Requisicao;

namespace SME.SR.JRSClient
{
    public static class JRSClientExtension
    {
        public static IServiceCollection AddJasperClient(this IServiceCollection services, string urlBase)
        {

            services.AddTransient<IInformacaoServidorService, InformacaoServidorService>();
            services.AddSingleton<Configuracoes>(new Configuracoes() { UrlBase = urlBase });

            return services;
        }
    }
}
