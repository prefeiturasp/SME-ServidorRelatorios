using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using SME.SR.IoC;
using SME.SR.Workers.SGP.Middlewares;
using SME.SR.Workers.SGP.Services;
using System.Linq;

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
            services.AddPolicies();

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
        }
    }
}
