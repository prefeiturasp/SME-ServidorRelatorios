using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Text;

namespace SME.SR.Infra
{
    public class FilaRabbit : IServicoFila
    {
        private readonly IModel rabbitChannel;

        public FilaRabbit(IModel rabbitChannel)
        {
            this.rabbitChannel = rabbitChannel ?? throw new ArgumentNullException(nameof(rabbitChannel));
        }

        public void PublicaFila(PublicaFilaDto publicaFilaDto)
        {
            if (!string.IsNullOrWhiteSpace(publicaFilaDto.Rota))
            {
                var request = new MensagemRabbit(publicaFilaDto.Rota.Replace(".","/"), publicaFilaDto.Dados, publicaFilaDto.CodigoCorrelacao);

                var mensagem = JsonConvert.SerializeObject(request);
                var body = Encoding.UTF8.GetBytes(mensagem);

                //TODO PENSAR NA EXCHANGE

                var exchange = publicaFilaDto.Exchange ?? RotasRabbit.ExchangeListenerWorkerRelatorios;

                rabbitChannel.QueueBind(publicaFilaDto.NomeFila, exchange, publicaFilaDto.Rota);

                rabbitChannel.BasicPublish(exchange, publicaFilaDto.Rota, null, body);
            }
        }
    }
}
