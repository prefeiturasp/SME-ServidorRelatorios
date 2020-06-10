using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using System;

namespace SME.SR.Workers.SGP.Configuracoes
{
    public static class ConfiguracaoRabbit
    {

       

        public static void AddRabbitMQ(this IServiceCollection services)
        {
            var factory = new ConnectionFactory
            {
                HostName = Environment.GetEnvironmentVariable("ConfiguracaoRabbit__Hostname"),
                UserName = Environment.GetEnvironmentVariable("ConfiguracaoRabbit__Username"),
                Password = Environment.GetEnvironmentVariable("ConfiguracaoRabbit__Password")
            };

            var conexaoRabbit = factory.CreateConnection();
            IModel channel = conexaoRabbit.CreateModel();

            services.AddSingleton(conexaoRabbit);
            services.AddSingleton(channel);
        }
    }
}
