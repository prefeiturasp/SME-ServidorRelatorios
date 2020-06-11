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
        public Task<bool> Handle(RetornarRelatorioProntoCommand request, CancellationToken cancellationToken)
        {
            servicoFila.PublicaFila(new PublicaFilaDto(request, RotasRabbit.FilaClientsSgp, RotasRabbit.RotaRelatoriosProntosSgp));
            return Task.FromResult(true);
        }
    }
}
