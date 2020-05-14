
using Microsoft.Extensions.DependencyInjection;
using SME.SR.JRSClient.Interfaces;
using SME.SR.JRSClient.Services;

namespace SME.SR.JRSClient
{
    public static class JRSClientExtension
    {
        public static IServiceCollection AddJasperClient(this IServiceCollection services, string urlBase, string jasperLogin, string jasperPassword)
        {

            services.AddTransient<IInformacaoServidorService, InformacaoServidorService>();
            services.AddTransient<ILoginService, LoginService>();

            services.AddSingleton<Configuracoes>(new Configuracoes()
            {
                UrlBase = urlBase,
                JasperLogin = jasperLogin,
                JasperPassword = jasperPassword
            });


            return services;
        }
    }
}
