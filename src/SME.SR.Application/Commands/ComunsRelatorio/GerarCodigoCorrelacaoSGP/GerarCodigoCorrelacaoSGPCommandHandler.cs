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
        public Task<Guid> Handle(GerarCodigoCorrelacaoSGPCommand request, CancellationToken cancellationToken)
        {

            var novaCorrelacao = Guid.NewGuid();

            servicoFila.PublicaFila(new PublicaFilaDto(novaCorrelacao, RotasRabbitSGP.RotaRelatorioCorrelacaoCopiar, ExchangeRabbit.Sgp, request.CodigoCorrelacaoParaCopiar));

            return Task.FromResult(novaCorrelacao);

        }
    }
}
