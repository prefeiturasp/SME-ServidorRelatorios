using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterPeriodoCompletoSondagemPorSemestreQueryHandler : IRequestHandler<ObterPeriodoCompletoSondagemPorSemestreQuery, PeriodoCompletoSondagemDto>
    {
        private readonly IPeriodoSondagemRepository periodoSondagemRepository;

        public ObterPeriodoCompletoSondagemPorSemestreQueryHandler(IPeriodoSondagemRepository periodoSondagemRepository)
        {
            this.periodoSondagemRepository = periodoSondagemRepository ?? throw new ArgumentNullException(nameof(periodoSondagemRepository));
        }
        public async Task<PeriodoCompletoSondagemDto> Handle(ObterPeriodoCompletoSondagemPorSemestreQuery request, CancellationToken cancellationToken)
            => await periodoSondagemRepository.ObterPeriodoCompletoPorSemestreEAnoLetivo(request.Semestre, request.AnoLetivo.ToString());
    }
}
