using MediatR;
using SME.SR.Infra;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application.Commands.RetornarRelatorioPronto
{
    public class RetornarRelatorioProntoCommandHandler : IRequestHandler<RetornarRelatorioProntoCommand, bool>
    {
        private readonly IServicoFila servicoFila;

        public RetornarRelatorioProntoCommandHandler(IServicoFila servicoFila)
        {
            this.servicoFila = servicoFila ?? throw new System.ArgumentNullException(nameof(servicoFila));
        }
        public async Task<bool> Handle(RetornarRelatorioProntoCommand request, CancellationToken cancellationToken)
        {
            await servicoFila.PublicaFila(new PublicaFilaDto(request, RotasRabbitSGP.RotaRelatoriosProntosSgp));
            return true;
        }
    }
}
