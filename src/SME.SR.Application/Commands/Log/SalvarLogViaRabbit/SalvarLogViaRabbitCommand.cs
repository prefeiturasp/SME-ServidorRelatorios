using MediatR;
using Newtonsoft.Json.Converters;
using SME.SR.Infra;
using System.Text.Json.Serialization;

namespace SME.SR.Application
{
    public class SalvarLogViaRabbitCommand : IRequest
    {
        public SalvarLogViaRabbitCommand(string mensagem, LogNivel nivel, string observacao = "")
        {
            Mensagem = mensagem;
            Nivel = nivel;
            Observacao = observacao;
        }

        public string Mensagem { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public LogNivel Nivel { get; set; }
        public string Observacao { get; set; }
    }
}
