using MediatR;
using SME.SR.Data;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterPeriodoPorTipoQueryHandler : IRequestHandler<ObterPeriodoPorTipoQuery, PeriodoSondagem>
    {
        private readonly IPeriodoSondagemRepository periodoSondagemRepository;

        public ObterPeriodoPorTipoQueryHandler(IPeriodoSondagemRepository periodoSondagemRepository)
        {
            this.periodoSondagemRepository = periodoSondagemRepository ?? throw new ArgumentNullException(nameof(periodoSondagemRepository));
        }

        public async Task<PeriodoSondagem> Handle(ObterPeriodoPorTipoQuery request, CancellationToken cancellationToken)
        {
            return await periodoSondagemRepository.ObterPeriodoPorTipo(request.Periodo, request.TipoPeriodo);
        }
    }
}
