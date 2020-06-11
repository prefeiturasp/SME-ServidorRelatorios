
using Microsoft.Extensions.DependencyInjection;
using SME.SR.JRSClient.Interfaces;
using SME.SR.JRSClient.Services;

namespace SME.SR.JRSClient
{
    public static class JRSClientExtension
    {
        public static IServiceCollection AddJasperClient(this IServiceCollection services, string urlBase, string jasperLogin, string jasperPassword)
        {
            services.AddSingleton(new Configuracoes()
            {
                UrlBase = urlBase,
                JasperLogin = jasperLogin,
                JasperPassword = jasperPassword
            });

            services.AddScoped<IInformacaoServidorService, InformacaoServidorService>();
            services.AddScoped<ILoginService, LoginService>();
            services.AddScoped<IControleEntradaService, ControleEntradaService>();
            services.AddScoped<IRelatorioService, RelatorioService>();
            services.AddScoped<ITrabalhoService, TrabalhoService>();
            services.AddScoped<IRecursoService, RecursoService>();
            //services.AddScoped<IExecucaoRelatorioService, ExecucaoRelatorioService>();

            //TODO FAZER ISSO FUNCIONAR E REMOVER DO STARTUP DO WORKER =/
            //services.AddHttpClient<IExecucaoRelatorioService, ExecucaoRelatorioService>(c =>
            //{
            //    c.BaseAddress = new Uri("http://127.0.0.1:8080");
            //})
            //      .ConfigurePrimaryHttpMessageHandler(() =>
            //      {
            //          return jasperCookieHandler;
            //      });



            return services;
        }
    }
}
