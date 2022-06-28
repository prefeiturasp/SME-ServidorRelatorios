using MediatR;
using Newtonsoft.Json;
using RabbitMQ.Client;
using SME.SR.Infra;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace SME.SR.Application
{
    public class SalvarLogViaRabbitCommandHandler : AsyncRequestHandler<SalvarLogViaRabbitCommand>
    {
        private readonly ConfiguracaoRabbitLogOptions configuracaoRabbitOptions;
        private readonly IConfiguration configuration;
        public SalvarLogViaRabbitCommandHandler(ConfiguracaoRabbitLogOptions configuracaoRabbitOptions, IConfiguration configuration)
        {
            this.configuracaoRabbitOptions = configuracaoRabbitOptions ?? throw new ArgumentNullException(nameof(configuracaoRabbitOptions));
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        protected override Task Handle(SalvarLogViaRabbitCommand request, CancellationToken cancellationToken)
        {
            var mensagem = JsonConvert.SerializeObject(new LogMensagem(request.Mensagem, request.Nivel.ToString(), "Relatorios", request.Observacao, "SR"), new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });

            var body = Encoding.UTF8.GetBytes(mensagem);

            PublicarMensagem(body, configuration);

            return Task.CompletedTask;
        }

        private void PublicarMensagem(byte[] body, IConfiguration configuration)
        {
            var factory = new ConnectionFactory
            {
                HostName = configuration.GetSection("configuracaoRabbitOptions:HostName").Value,
                UserName = configuration.GetSection("configuracaoRabbitOptions:UserName").Value,
                Password = configuration.GetSection("configuracaoRabbitOptions:Password").Value,
                VirtualHost = configuration.GetSection("configuracaoRabbitOptions:Virtualhost").Value
            };

            using (var conexaoRabbit = factory.CreateConnection())
            {
                using (IModel _channel = conexaoRabbit.CreateModel())
                {
                    var props = _channel.CreateBasicProperties();
                    _channel.BasicPublish(ExchangeRabbit.SgpLogs, RotasRabbitSGP.RotaLogs, props, body);
                }
            }
        }

        public class LogMensagem
        {
            public LogMensagem(string mensagem, string nivel, string contexto, string observacao, string projeto)
            {
                Mensagem = mensagem;
                Nivel = nivel;
                Contexto = contexto;
                Observacao = observacao;
                Projeto = projeto;
                DataHora = DateTime.Now;
            }

            public string Mensagem { get; set; }
            public string Nivel { get; set; }
            public string Contexto { get; set; }
            public string Observacao { get; set; }
            public string Projeto { get; set; }
            public DateTime DataHora { get; set; }

        }
    }
}
