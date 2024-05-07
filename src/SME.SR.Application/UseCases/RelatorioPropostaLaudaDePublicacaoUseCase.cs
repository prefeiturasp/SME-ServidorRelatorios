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

        public Task<string> Executar(long propostaId)
        {
            throw new NotImplementedException();
        }
    }
}
