using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Data.Models.Conecta;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterPropostaCompletaQueryHandler : IRequestHandler<ObterPropostaCompletaQuery, PropostaCompleta>
    {
        private readonly IPropostaRepository repositorio;

        public ObterPropostaCompletaQueryHandler(IPropostaRepository repositorio)
        {
            this.repositorio = repositorio ?? throw new ArgumentNullException(nameof(repositorio));
        }

        public Task<PropostaCompleta> Handle(ObterPropostaCompletaQuery request, CancellationToken cancellationToken)
        {
            return repositorio.ObterPropostaCompleta(request.PropostaId);
        }
    }
}
