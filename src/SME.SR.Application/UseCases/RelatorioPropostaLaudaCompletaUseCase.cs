using MediatR;
using SME.SR.Application.Interfaces;
using System;
using System.Threading.Tasks;

namespace SME.SR.Application.UseCases
{
    public class RelatorioPropostaLaudaCompletaUseCase : IRelatorioPropostaLaudaCompletaUseCase
    {
        private readonly IMediator mediator;

        public RelatorioPropostaLaudaCompletaUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<string> Executar(long propostaId)
        {
            var proposta = await mediator.Send(new ObterPropostaCompletaQuery(propostaId));

            if (proposta == null || proposta.Id == 0)
                return string.Empty;

            throw new NotImplementedException();
        }
    }
}
