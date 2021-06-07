using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Text;

namespace SME.SR.Infra
{
    public class FilaRabbit : IServicoFila
    {
        private readonly IConfiguration configuration;

        public FilaRabbit(IConfiguration configuration)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public void PublicaFila(PublicaFilaDto publicaFilaDto)
        {

            var request = new MensagemRabbit(publicaFilaDto.Rota.Replace(".", "/"), publicaFilaDto.Dados, publicaFilaDto.CodigoCorrelacao);

            var mensagem = JsonConvert.SerializeObject(request, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            var body = Encoding.UTF8.GetBytes(mensagem);

            var factory = new ConnectionFactory
            {
                HostName = configuration.GetSection("ConfiguracaoRabbit:HostName").Value,
                UserName = configuration.GetSection("ConfiguracaoRabbit:UserName").Value,
                Password = configuration.GetSection("ConfiguracaoRabbit:Password").Value,
                VirtualHost = configuration.GetSection("ConfiguracaoRabbit:Virtualhost").Value
            };

            var exchange = publicaFilaDto.Exchange ?? RotasRabbit.ExchangeListenerWorkerRelatorios;

            using (var conexaoRabbit = factory.CreateConnection())
            {
                using (IModel _channel = conexaoRabbit.CreateModel())
                {
                    _channel.BasicPublish(exchange, publicaFilaDto.Rota, null, body);
                }
            }
        }
    }
}
