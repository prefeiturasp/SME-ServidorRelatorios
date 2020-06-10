using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using SME.SR.Application;
using SME.SR.Data;
using SME.SR.Infra;
using SME.SR.JRSClient;
using SME.SR.JRSClient.Extensions;
using SME.SR.JRSClient.Interfaces;
using SME.SR.JRSClient.Services;
using SME.SR.Workers.SGP.Configuracoes;
using SME.SR.Workers.SGP.Services;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace SME.SR.Workers.SGP
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddResponseCompression(options =>
            {
                options.Providers.Add<GzipCompressionProvider>();
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/json" });
            });
            var assembly = AppDomain.CurrentDomain.Load("SME.SR.Application");
            services.AddMediatR(assembly);

            services.AddControllers();
            services.AddMvc().AddControllersAsServices();
            services.AddRabbitMQ();
            services.AddHostedService<RabbitBackgroundListener>();

            //TODO: Informa�oes do arquivo de configura��o

            var cookieContainer = new CookieContainer();
            var jasperCookieHandler = new JasperCookieHandler() { CookieContainer = cookieContainer };

            services.AddSingleton(jasperCookieHandler);

            services.AddHttpClient<IExecucaoRelatorioService, ExecucaoRelatorioService>(c =>
            {
                c.BaseAddress = new Uri("http://127.0.0.1:8080");
            })
                .ConfigurePrimaryHttpMessageHandler(() =>
                {
                    return jasperCookieHandler;
                });

            services.AddJasperClient("http://127.0.0.1:8080", "user", "bitnami");

            services.AddSingleton(new VariaveisAmbiente());

            // TODO: Criar arquivo especficio para as inje��es
            RegistrarRepositorios(services);
            RegistrarUseCase(services);
            RegistrarServicos(services);
        }

        private void RegistrarRepositorios(IServiceCollection services)
        {
            services.TryAddScoped<IExemploRepository, ExemploRepository>();
            services.TryAddScoped<IRelatorioGamesUseCase, RelatorioGamesUseCase>();
            services.TryAddScoped<IMonitorarStatusRelatorioUseCase, MonitorarStatusRelatorioUseCase>();
        }

        private void RegistrarUseCase(IServiceCollection services)
        {
            services.TryAddScoped<IRelatorioGamesUseCase, RelatorioGamesUseCase>();
        }

        private void RegistrarServicos(IServiceCollection services)
        {
            services.TryAddScoped<IServicoFila, FilaRabbit>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app
                .UseCors(x => x
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod())
                .UseRouting()
                .UseAuthorization()
                .UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }
    }
}
