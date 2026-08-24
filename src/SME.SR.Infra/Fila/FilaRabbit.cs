using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Polly;
using Polly.Registry;
using RabbitMQ.Client;
using System;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Infra
{
    public class FilaRabbit : IServicoFila
    {
        private readonly IConfiguration configuration;
        private readonly IAsyncPolicy policy;

        public FilaRabbit(IConfiguration configuration, IReadOnlyPolicyRegistry<string> registry)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this.policy = registry.Get<IAsyncPolicy>(PoliticaPolly.PublicaFila);
        }

        public async Task PublicaFila(PublicaFilaDto publicaFilaDto)
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
                VirtualHost = configuration.GetSection("ConfiguracaoRabbit:Virtualhost").Value,
                AutomaticRecoveryEnabled = true
            };
            await policy.ExecuteAsync(() => PublicaMensagem(publicaFilaDto, body, factory));
        }
        private  static Task PublicaMensagem(PublicaFilaDto publicaFilaDto, byte[] body, ConnectionFactory factory)
        {
            try
            {
                var exchange = publicaFilaDto.Exchange ?? ExchangeRabbit.WorkerRelatorios;

                using (var conexaoRabbit = factory.CreateConnection())
                using (var channel = conexaoRabbit.CreateModel())
                {
                    var props = channel.CreateBasicProperties();

                    channel.BasicPublish(exchange, publicaFilaDto.Rota, false, props, body);
                }

                return Task.CompletedTask;
            }
            catch (Exception)
            {
                return Task.Delay(5000);
            }
        }
    }
}
