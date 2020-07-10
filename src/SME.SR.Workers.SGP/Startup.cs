using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using SME.SR.IoC;
using SME.SR.Workers.SGP.Middlewares;
using SME.SR.Workers.SGP.Services;
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

            services.AddDirectoryBrowser();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "SME - Servidor de relatórios", Version = "v1" });
            });

        }

        //private void RegistraServicosHttp(IServiceCollection services)
        //{
        //    var cookieContainer = new CookieContainer();
        //    var jasperCookieHandler = new JasperCookieHandler() { CookieContainer = cookieContainer };

        //    services.AddSingleton(jasperCookieHandler);

        //    var basicAuth = $"{Configuration.GetValue<string>("ConfiguracaoJasper:Username")}:{Configuration.GetValue<string>("ConfiguracaoJasper:Password")}".EncodeTo64();
        //    var jasperUrl = Configuration.GetValue<string>("ConfiguracaoJasper:Hostname");

        //    services.AddHttpClient<GerarRelatorioAssincronoCommandHandler>(c =>
        //    {
        //        c.BaseAddress = new Uri(jasperUrl);
        //        c.DefaultRequestHeaders.Add("Accept", "application/json");
        //        c.DefaultRequestHeaders.Add("Authorization", $"Basic {basicAuth}");
        //    }).ConfigurePrimaryHttpMessageHandler(() =>
        //    {
        //        return new JasperCookieHandler() { CookieContainer = cookieContainer };
        //    });
        //}

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "SME - Servidor de relatórios");
            });

            app.UseMiddleware<ExcecaoMiddleware>();

            app.UseStaticFiles(new StaticFileOptions
            {
                ServeUnknownFileTypes = true,
                DefaultContentType = "text/html"
            });

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
