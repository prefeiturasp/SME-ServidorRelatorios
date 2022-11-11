using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;

namespace SME.SR.Application
{
    public class ObterPeriodoEscolarPorIdQueryHandler : IRequestHandler<ObterPeriodoEscolarPorIdQuery, PeriodoEscolar>
    {
        private readonly IPeriodoEscolarRepository periodoEscolarRepository;

        public ObterPeriodoEscolarPorIdQueryHandler(IPeriodoEscolarRepository periodoEscolarRepository)
        {
            this.periodoEscolarRepository = periodoEscolarRepository ?? throw new ArgumentNullException(nameof(periodoEscolarRepository));
        }

        public async Task<PeriodoEscolar> Handle(ObterPeriodoEscolarPorIdQuery request, CancellationToken cancellationToken)
        {
            return await periodoEscolarRepository.ObterPeriodoEscolarPorId(request.IdPeriodoEscolar);
        }
    }
}