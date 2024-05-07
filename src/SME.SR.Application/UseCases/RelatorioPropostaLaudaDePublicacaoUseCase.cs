using MediatR;
using SME.SR.Application.Interfaces;
using System;
using System.Threading.Tasks;

namespace SME.SR.Application.UseCases
{
    public class RelatorioPropostaLaudaDePublicacaoUseCase : IRelatorioPropostaLaudaDePublicacaoUseCase
    {
        private readonly IMediator mediator;

        public RelatorioPropostaLaudaDePublicacaoUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<string> Executar(long propostaId)
        {
            var proposta = await mediator.Send(new ObterPropostaQuery(propostaId));

            return string.Empty;
        }
    }
}
