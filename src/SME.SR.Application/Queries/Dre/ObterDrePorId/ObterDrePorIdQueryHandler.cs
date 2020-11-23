using MediatR;
using SME.SR.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterDrePorIdQueryHandler : IRequestHandler<ObterDrePorIdQuery, Dre>
    {
        private readonly IDreRepository dreRepository;

        public ObterDrePorIdQueryHandler(IDreRepository dreRepository)
        {
            this.dreRepository = dreRepository ?? throw new ArgumentNullException(nameof(dreRepository));
        }

        public async Task<Dre> Handle(ObterDrePorIdQuery request, CancellationToken cancellationToken)
            => await dreRepository.ObterPorId(request.DreId);
    }
}
