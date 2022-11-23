using Elastic.Apm.AspNetCore;
using Elastic.Apm.DiagnosticSource;
using Elastic.Apm.SqlClient;
using HealthChecks.UI.Client;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using RabbitMQ.Client;
using SME.SR.Data;
using SME.SR.Infra;
using SME.SR.Infra.Extensions;
using SME.SR.IoC;
using SME.SR.Workers.SGP.Filters;
using SME.SR.Workers.SGP.Middlewares;
using SME.SR.Workers.SGP.Services;
using System;
using System.Linq;
using System.Net.Mime;
using System.Text.Json;
using System.Web;

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

            services.AddTransient<ExcecaoMiddleware>();

            services.AddApplicationInsightsTelemetry(Configuration);

            services.RegistrarDependencias(Configuration);

            ConfiguraRabbitParaLogs(services);
            ConfiguraTelemetria(services);

            services.AddDirectoryBrowser();
            services.AddPolicies();

            services.AddHostedService<RabbitBackgroundListener>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "SME - Servidor de relatórios", Version = "v1" });
                c.OperationFilter<FiltroIntegracao>();
            });


            services.AddHealthChecks()
               .AddPostgres(Configuration)
               .AddPostgresConsultas(Configuration)
               .AddRabbitMQ(Configuration)
               .AddRabbitMQLog(Configuration);
            services.AddHealthChecksUI();
        }


        private void ConfiguraRabbitParaLogs(IServiceCollection services)
        {
            var configuracaoRabbitLogOptions = new ConfiguracaoRabbitLogOptions();
            Configuration.GetSection("ConfiguracaoRabbitLog").Bind(configuracaoRabbitLogOptions, c => c.BindNonPublicProperties = true);

            services.AddSingleton(configuracaoRabbitLogOptions);
        }

        private void ConfiguraTelemetria(IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var clientTelemetry = serviceProvider.GetService<TelemetryClient>();

            var telemetriaOptions = new TelemetriaOptions();
            Configuration.GetSection(TelemetriaOptions.Secao).Bind(telemetriaOptions, c => c.BindNonPublicProperties = true);

            IServicoTelemetria servicoTelemetria = new ServicoTelemetria(clientTelemetry, telemetriaOptions);
            DapperExtensionMethods.Init(servicoTelemetria);

            services.AddSingleton(telemetriaOptions);
            services.AddSingleton<IServicoTelemetria, ServicoTelemetria>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseElasticApm(Configuration,
                new SqlClientDiagnosticSubscriber(),
                new HttpDiagnosticsSubscriber());

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "SME - Servidor de relatórios");
            });

            app.UseMiddleware<ExcecaoMiddleware>();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHttpsRedirection();
            }

            app.UseStaticFiles();

            app.UseFileServer(enableDirectoryBrowsing: false);

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

            app.UseHealthChecks("/healthz", new HealthCheckOptions()
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
            app.UseHealthChecksUI();
        }
    }
}
