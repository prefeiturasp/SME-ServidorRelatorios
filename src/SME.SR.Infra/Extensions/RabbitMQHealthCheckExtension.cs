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
            var configurationSection = "ConfiguracaoRabbitLog";
            var userName = HttpUtility.UrlEncode(configuration.GetSection(configurationSection + ":Username").Value);
            var password = HttpUtility.UrlEncode(configuration.GetSection(configurationSection + ":Password").Value);
            var hostName = configuration.GetSection(configurationSection+":Hostname").Value;
            var vHost = HttpUtility.UrlEncode(configuration.GetSection(configurationSection+":Virtualhost").Value);

            string connectionString = $"amqp://{userName}:{password}@{hostName}/{vHost}";
            return builder.AddRabbitMQ(connectionString,
            name: "RabbitMQ Log",
            failureStatus: HealthStatus.Unhealthy);
        }

        public static IHealthChecksBuilder AddRabbitMQ(this IHealthChecksBuilder builder, IConfiguration configuration)
        {
            var configurationSection = "ConfiguracaoRabbit";
            var userName = HttpUtility.UrlEncode(configuration.GetSection(configurationSection + ":Username").Value);
            var password = HttpUtility.UrlEncode(configuration.GetSection(configurationSection + ":Password").Value);
            var hostName = configuration.GetSection(configurationSection + ":Hostname").Value;
            var vHost = HttpUtility.UrlEncode(configuration.GetSection(configurationSection + ":Virtualhost").Value);

            string connectionString = $"amqp://{userName}:{password}@{hostName}/{vHost}";
            return builder.AddRabbitMQ(connectionString,
            name: "RabbitMQ",
            failureStatus: HealthStatus.Unhealthy);
        }

    }
}
