using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SME.SR.Data;
using SME.SR.Workers.SGP.Commands;
using SME.SR.Workers.SGP.Commons.Interfaces.Repositories;
using SME.SR.JRSClient;
using SME.SR.Workers.SGP.Services;

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

            services.AddMediatR(Assembly.GetExecutingAssembly());

            services.AddControllers();
            services.AddMvc().AddControllersAsServices();
            services.AddHostedService<RabbitBackgroundListener>();
            
            //TODO: Informaçoes do arquivo de configuração
            services.AddJasperClient("http://127.0.0.1:8080", "user", "bitnami");

            // TODO: Criar arquivo especficio para as injeções
            RegistrarRepositorios(services);            

            
        }

        private void RegistrarRepositorios(IServiceCollection services)
        {
            services.TryAddScoped<IExemploRepository, ExemploRepository>();
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
