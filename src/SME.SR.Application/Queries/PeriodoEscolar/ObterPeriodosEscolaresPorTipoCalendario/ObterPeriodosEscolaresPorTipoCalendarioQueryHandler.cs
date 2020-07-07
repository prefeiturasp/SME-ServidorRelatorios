using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterPeriodosEscolaresPorTipoCalendarioQueryHandler : IRequestHandler<ObterPeriodosEscolaresPorTipoCalendarioQuery, IEnumerable<PeriodoEscolar>>
    {
        private readonly IPeriodoEscolarRepository periodoEscolarRepository;

        public ObterPeriodosEscolaresPorTipoCalendarioQueryHandler(IPeriodoEscolarRepository periodoEscolarRepository)
        {
            this.periodoEscolarRepository = periodoEscolarRepository ?? throw new ArgumentNullException(nameof(periodoEscolarRepository));
        }

        public async Task<IEnumerable<PeriodoEscolar>> Handle(ObterPeriodosEscolaresPorTipoCalendarioQuery request, CancellationToken cancellationToken)
            => await periodoEscolarRepository.ObterPeriodosEscolaresPorTipoCalendario(request.TipoCalendarioId);
    }
}
