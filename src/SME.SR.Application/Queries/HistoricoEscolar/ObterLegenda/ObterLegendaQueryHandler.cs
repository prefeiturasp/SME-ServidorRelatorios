using System;
using System.Collections.Generic;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;

namespace SME.SR.Application
{
    public class ObterLegendaQueryHandler : IRequestHandler<ObterLegendaQuery, IEnumerable<ConceitoDto>>
    {
        private readonly IConceitoValoresRepository _conceitoValoresRepository;

        public ObterLegendaQueryHandler(IConceitoValoresRepository conceitoValoresRepository)
        {
            this._conceitoValoresRepository = conceitoValoresRepository ?? throw new ArgumentNullException(nameof(conceitoValoresRepository));
        }

        public async Task<IEnumerable<ConceitoDto>> 
            Handle(ObterLegendaQuery request, CancellationToken cancellationToken)
        {
            return await _conceitoValoresRepository.ObterDadosLegendaHistoricoEscolar();
        }
    }
}
