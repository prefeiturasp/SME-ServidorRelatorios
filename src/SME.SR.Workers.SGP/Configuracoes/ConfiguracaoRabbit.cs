using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using System;

namespace SME.SR.Workers.SGP.Configuracoes
{
    public static class ConfiguracaoRabbit
    {

       

        public static void AddRabbitMQ(this IServiceCollection services, IConfiguration configuration)
        {
            var factory = new ConnectionFactory
            {
                HostName = configuration.GetValue<string>("ConfiguracaoRabbit:HostName"),
                UserName = configuration.GetValue<string>("ConfiguracaoRabbit:UserName"),
                Password = configuration.GetValue<string>("ConfiguracaoRabbit:Password")
            };

            var conexaoRabbit = factory.CreateConnection();
            IModel channel = conexaoRabbit.CreateModel();

            services.AddSingleton(conexaoRabbit);
            services.AddSingleton(channel);
        }
    }
}
