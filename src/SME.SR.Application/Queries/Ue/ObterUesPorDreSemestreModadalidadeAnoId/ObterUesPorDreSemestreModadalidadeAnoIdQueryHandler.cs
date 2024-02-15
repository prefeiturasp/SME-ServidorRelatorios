using MediatR;
using SME.SR.Data;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterUesPorDreSemestreModadalidadeAnoIdQueryHandler : IRequestHandler<ObterUesPorDreSemestreModadalidadeAnoIdQuery, IEnumerable<Ue>>
    {
        private readonly IUeRepository ueRepository;

        public ObterUesPorDreSemestreModadalidadeAnoIdQueryHandler(IUeRepository ueRepository)
        {
            this.ueRepository = ueRepository ?? throw new ArgumentNullException(nameof(UeRepository));
        }
        public async Task<IEnumerable<Ue>> Handle(ObterUesPorDreSemestreModadalidadeAnoIdQuery request, CancellationToken cancellationToken)
        {
            return await ueRepository.ObterPorDreSemestreModadalidadeAnoId(request.DreId, request.Semestre, request.ModalidadeId, request.Ano, request.TipoEscolas);
        }
    }
}
