using MediatR;
using Newtonsoft.Json;
using RabbitMQ.Client;
using SME.SR.Infra;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class SalvarLogViaRabbitCommandHandler : AsyncRequestHandler<SalvarLogViaRabbitCommand>
    {
        private readonly ConfiguracaoRabbitLogOptions configuracaoRabbitOptions;
        public SalvarLogViaRabbitCommandHandler(ConfiguracaoRabbitLogOptions configuracaoRabbitOptions)
        {
            this.configuracaoRabbitOptions = configuracaoRabbitOptions ?? throw new ArgumentNullException(nameof(configuracaoRabbitOptions));
        }

        protected override Task Handle(SalvarLogViaRabbitCommand request, CancellationToken cancellationToken)
        {
            var mensagem = JsonConvert.SerializeObject(new LogMensagem(request.Mensagem, request.Nivel.ToString(), "Relatorios", request.Observacao, "SR"), new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });

            var body = Encoding.UTF8.GetBytes(mensagem);

            PublicarMensagem(body);

            return Task.CompletedTask;
        }

        private void PublicarMensagem(byte[] body)
        {
            var factory = new ConnectionFactory
            {
                HostName = configuracaoRabbitOptions.HostName,
                UserName = configuracaoRabbitOptions.UserName,
                Password = configuracaoRabbitOptions.Password,
                VirtualHost = configuracaoRabbitOptions.VirtualHost
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
