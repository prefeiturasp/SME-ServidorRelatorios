
using Microsoft.Extensions.DependencyInjection;
using SME.SR.JRSClient.Interfaces;
using SME.SR.JRSClient.Services;

namespace SME.SR.JRSClient
{
    public static class JRSClientExtension
    {
        public static IServiceCollection AddJasperClient(this IServiceCollection services, string urlBase, string jasperLogin, string jasperPassword)
        {

            services.AddScoped<IInformacaoServidorService, InformacaoServidorService>();
            services.AddScoped<ILoginService, LoginService>();
            services.AddScoped<IControleEntradaService, ControleEntradaService>();
            services.AddScoped<IRelatorioService, RelatorioService>();
            services.AddScoped<ITrabalhoService, TrabalhoService>();
            services.AddScoped<IRecursoService, RecursoService>();
            services.AddScoped<IExecucaoRelatorioService, ExecucaoRelatorioService>();

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
