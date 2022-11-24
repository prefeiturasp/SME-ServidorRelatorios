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
    public static class RabbitMQHealthCheckExtension
    {

        public static IHealthChecksBuilder AddRabbitMQLog(this IHealthChecksBuilder builder, IConfiguration configuration)
        {
            return builder.AddRabbitMQ(ObterStringConexao(configuration, "ConfiguracaoRabbitLog"),
                                       name: "RabbitMQ Log",
                                       failureStatus: HealthStatus.Unhealthy);
        }

        public static IHealthChecksBuilder AddRabbitMQ(this IHealthChecksBuilder builder, IConfiguration configuration)
        {
            return builder.AddRabbitMQ(ObterStringConexao(configuration, "ConfiguracaoRabbit"),
                                       name: "RabbitMQ",
                                       failureStatus: HealthStatus.Unhealthy);
        }

        private static string ObterStringConexao(IConfiguration configuration, string configSection)
        {
            var userName = HttpUtility.UrlEncode(configuration.GetSection(configSection + ":Username").Value);
            var password = HttpUtility.UrlEncode(configuration.GetSection(configSection + ":Password").Value);
            var hostName = configuration.GetSection(configSection + ":Hostname").Value;
            var vHost = HttpUtility.UrlEncode(configuration.GetSection(configSection + ":Virtualhost").Value);

            return $"amqp://{userName}:{password}@{hostName}/{vHost}";
        }

    }
}
