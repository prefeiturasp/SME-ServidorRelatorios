using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterRelatorioImpressaoCalendarioQueryHandler : IRequestHandler<ObterRelatorioImpressaoCalendarioQuery, RelatorioImpressaoCalendarioDto>
    {
        private readonly ICalendarioEventoRepository calendarioEventoRepository;

        public ObterRelatorioImpressaoCalendarioQueryHandler(ICalendarioEventoRepository calendarioEventoRepository)
        {
            this.calendarioEventoRepository = calendarioEventoRepository ?? throw new ArgumentNullException(nameof(calendarioEventoRepository));
        }
        public Task<RelatorioImpressaoCalendarioDto> Handle(ObterRelatorioImpressaoCalendarioQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
