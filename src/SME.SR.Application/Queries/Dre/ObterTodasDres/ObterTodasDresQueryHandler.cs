using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterTodasDresQueryHandler : IRequestHandler<ObterTodasDresQuery, IEnumerable<Dre>>
    {
        private readonly IDreRepository dreRepository;

        public ObterTodasDresQueryHandler(IDreRepository dreRepository)
        {
            this.dreRepository = dreRepository ?? throw new ArgumentNullException(nameof(dreRepository));
        }
        public async Task<IEnumerable<Dre>> Handle(ObterTodasDresQuery request, CancellationToken cancellationToken)
        {
            return await dreRepository.ObterTodas();            
        }
    }
}
