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

        private readonly IAsyncPolicy policy;
        private readonly IModel rabbitConnection;

        public FilaRabbit(IReadOnlyPolicyRegistry<string> registry, IModel rabbitConnection)
        {

            this.policy = registry.Get<IAsyncPolicy>(PoliticaPolly.PublicaFila);
            this.rabbitConnection = rabbitConnection ?? throw new ArgumentNullException(nameof(rabbitConnection));
        }

        public async Task PublicaFila(PublicaFilaDto publicaFilaDto)
        {

            var request = new MensagemRabbit(publicaFilaDto.Rota.Replace(".", "/"), publicaFilaDto.Dados, publicaFilaDto.CodigoCorrelacao);

            var mensagem = JsonConvert.SerializeObject(request, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            var body = Encoding.UTF8.GetBytes(mensagem);

            await policy.ExecuteAsync(() => PublicaMensagem(publicaFilaDto, body));
        }

        private async Task PublicaMensagem(PublicaFilaDto publicaFilaDto, byte[] body)
        {
            var exchange = publicaFilaDto.Exchange ?? ExchangeRabbit.WorkerRelatorios;

            rabbitConnection.BasicPublish(exchange, publicaFilaDto.Rota, null, body);

            await Task.FromResult(true);
        }
    }
}
