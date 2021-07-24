using MediatR;
using SME.SR.Infra;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class GerarCodigoCorrelacaoSGPCommandHandler : IRequestHandler<GerarCodigoCorrelacaoSGPCommand, Guid>
    {
        private readonly IServicoFila servicoFila;
        public GerarCodigoCorrelacaoSGPCommandHandler(IServicoFila servicoFila)
        {
            this.servicoFila = servicoFila ?? throw new ArgumentNullException(nameof(servicoFila));
        }
        public async Task<Guid> Handle(GerarCodigoCorrelacaoSGPCommand request, CancellationToken cancellationToken)
        {

            var novaCorrelacao = Guid.NewGuid();

            await servicoFila.PublicaFila(new PublicaFilaDto(novaCorrelacao, RotasRabbit.RotaRelatorioCorrelacaoCopiar, RotasRabbit.ExchangeSgp, request.CodigoCorrelacaoParaCopiar));

            return novaCorrelacao;

        }
    }
}
