using HtmlAgilityPack;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using RabbitMQ.Client;
using System.Collections.Generic;
using System;
using Microsoft.Extensions.Configuration;
using System.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Elastic.Apm.Api;
using HealthChecks.UI.Client;

namespace SME.SR.Infra.Extensions
{
    public static class HealthCheckExtensions
    {

        public static IApplicationBuilder UseHealthChecksGen(this IApplicationBuilder app)
        {
            return app.UseHealthChecks("/healthz", new HealthCheckOptions()
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            }).UseHealthChecksUI();
        }

        public static IServiceCollection AddHealthChecksGen(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHealthChecks()
                .AddPostgres(configuration)
                .AddPostgresConsultas(configuration)
                .AddRabbitMQ(configuration)
                .AddRabbitMQLog(configuration);
            services.AddHealthChecksUI("healthchecksdb", options =>
                                    {
                                        options.SetEvaluationTimeInSeconds(5);
                                        options.AddHealthCheckEndpoint("Health-API Indicadores", "/healthz");
                                    });
            return services;
        }

    }
}
