using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Data.Models.Conecta;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterPropostaQueryHandler : IRequestHandler<ObterPropostaQuery, Proposta>
    {
        private readonly IPropostaRepository repositorio;

        public ObterPropostaQueryHandler(IPropostaRepository repositorio)
        {
            this.repositorio = repositorio ?? throw new ArgumentNullException(nameof(repositorio));
        }

        public Task<Proposta> Handle(ObterPropostaQuery request, CancellationToken cancellationToken)
        {
            return this.repositorio.ObterProposta(request.PropostaId);
        }
    }
}
