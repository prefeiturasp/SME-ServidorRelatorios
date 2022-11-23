using HtmlAgilityPack;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using RabbitMQ.Client;
using System.Collections.Generic;
using System;
using Microsoft.Extensions.Configuration;
using System.Web;

namespace SME.SR.Infra.Extensions
{
    public static class PostgresHealthCheckExtension
    {

        public static IHealthChecksBuilder AddPostgres(this IHealthChecksBuilder builder, IConfiguration configuration)
        {
            return builder.AddNpgSql(configuration.GetConnectionString("SGP_Postgres"),
            name: "Postgres",
            failureStatus: HealthStatus.Unhealthy);
        }

        public static IHealthChecksBuilder AddPostgresConsultas(this IHealthChecksBuilder builder, IConfiguration configuration)
        {
            return builder.AddNpgSql(configuration.GetConnectionString("SGP_PostgresConsultas"),
            name: "Postgres Consultas",
            failureStatus: HealthStatus.Unhealthy);
        }

    }
}
