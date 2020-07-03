using DinkToPdf;
using DinkToPdf.Contracts;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using SME.SR.IoC;
using SME.SR.JRSClient;
using SME.SR.JRSClient.Extensions;
using SME.SR.JRSClient.Interfaces;
using SME.SR.JRSClient.Services;
using SME.SR.Workers.SGP.Configuracoes;
using SME.SR.Workers.SGP.Middlewares;
using SME.SR.Workers.SGP.Services;
using System;
using System.Linq;
using System.Net;

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

            services.AddControllers();
            services.AddMvc().AddControllersAsServices();
            services.AddHostedService<RabbitBackgroundListener>();
            services.AddTransient<ExcecaoMiddleware>();
            services.RegistrarDependencias(Configuration);

            //var cookieContainer = new CookieContainer();
            //var jasperCookieHandler = new JasperCookieHandler() { CookieContainer = cookieContainer };

            //services.AddSingleton(jasperCookieHandler);

            //var urlJasper = Configuration.GetValue<string>("ConfiguracaoJasper:Hostname");
            //var usuarioJasper = Configuration.GetValue<string>("ConfiguracaoJasper:Username");
            //var senhaJasper = Configuration.GetValue<string>("ConfiguracaoJasper:Password");


            //services.AddHttpClient<IExecucaoRelatorioService, ExecucaoRelatorioService>(c =>
            //{
            //    c.BaseAddress = new Uri(urlJasper);
            //})
            //    .ConfigurePrimaryHttpMessageHandler(() =>
            //    {
            //        return new JasperCookieHandler() { CookieContainer = cookieContainer };
            //    });

            //services.AddJasperClient(urlJasper, usuarioJasper, senhaJasper);


            services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));


            services.AddDirectoryBrowser();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "SME - Servidor de relatórios", Version = "v1" });
            });

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "SME - Servidor de relatórios");
            });

            app.UseMiddleware<ExcecaoMiddleware>();

            app.UseStaticFiles();

            app.UseFileServer(enableDirectoryBrowsing: true);

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
            app.UsePathBase("/worker-relatorios");
        }
    }
}
