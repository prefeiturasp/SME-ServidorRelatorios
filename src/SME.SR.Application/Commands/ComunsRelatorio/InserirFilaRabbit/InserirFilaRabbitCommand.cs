using MediatR;
using SME.SR.Infra;

namespace SME.SR.Application
{
    public class InserirFilaRabbitCommand : IRequest<bool>
    {
        public InserirFilaRabbitCommand(PublicaFilaDto adicionarFilaDto)
        {
            AdicionarFilaDto = adicionarFilaDto;
        }

        public PublicaFilaDto AdicionarFilaDto { get; set; }
    }
}
