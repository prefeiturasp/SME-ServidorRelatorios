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

            if (proposta == null || proposta.Id == 0)
                return string.Empty;

            if (proposta.EhAreaPromotoraDireta)
                return await mediator.Send(new GerarRelatorioLaudaDePublicacaoDiretaDocCommand(proposta));

            return await mediator.Send(new GerarRelatorioLaudaDePublicacaoParceiraDocCommand(proposta)); 
        }
    }
}
