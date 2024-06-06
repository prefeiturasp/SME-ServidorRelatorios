using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterResumoBuscasAtivasQueryHandler : IRequestHandler<ObterResumoBuscasAtivasQuery, IEnumerable<BuscaAtivaSimplesDto>>
    {
        private readonly IBuscaAtivaRepository registroRepository;

        public ObterResumoBuscasAtivasQueryHandler(IBuscaAtivaRepository registroRepository)
        {
            this.registroRepository = registroRepository ?? throw new ArgumentNullException(nameof(registroRepository));
        }

        public Task<IEnumerable<BuscaAtivaSimplesDto>> Handle(ObterResumoBuscasAtivasQuery request, CancellationToken cancellationToken)
        {
            return registroRepository.ObterResumoBuscasAtivas(request.Filtro);   
        }
    }
}
