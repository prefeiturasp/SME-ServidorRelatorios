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
    public class InserirFilaRabbitCommandHandler : IRequestHandler<InserirFilaRabbitCommand, bool>
    {
        private readonly IModel rabbitChannel;

        public InserirFilaRabbitCommandHandler(IModel rabbitChannel)
        {
            this.rabbitChannel = rabbitChannel ?? throw new ArgumentNullException(nameof(rabbitChannel));
        }

        public Task<bool> Handle(InserirFilaRabbitCommand request, CancellationToken cancellationToken)
        {
            byte[] body = FormataBodyWorker(request.AdicionarFilaDto);

            rabbitChannel.BasicPublish(request.AdicionarFilaDto.Exchange, request.AdicionarFilaDto.Rota, null, body);

            return Task.FromResult(true);
        }

        private static byte[] FormataBodyWorker(PublicaFilaDto adicionaFilaDto)
        {
            var request = new MensagemRabbit(adicionaFilaDto.Rota, adicionaFilaDto.Dados, adicionaFilaDto.CodigoCorrelacao, adicionaFilaDto.UsuarioLogadoRF);
            var mensagem = JsonConvert.SerializeObject(request);
            var body = Encoding.UTF8.GetBytes(mensagem);
            return body;
        }
    }
}
