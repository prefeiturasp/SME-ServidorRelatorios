using SME.SR.Application.Queries;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using MediatR;
using SME.SR.Infra.Dtos.Sondagem;

namespace SME.SR.Application
{
    public class ObterPeriodoCompletoSondagemPorBimestreQueryHandler : IRequestHandler<ObterPeriodoCompletoSondagemPorBimestreQuery, PeriodoCompletoSondagemDto>
    {

        private readonly IPeriodoSondagemRepository periodoSondagemRepository;

        public ObterPeriodoCompletoSondagemPorBimestreQueryHandler(IPeriodoSondagemRepository periodoSondagemRepository)
        {
            this.periodoSondagemRepository = periodoSondagemRepository ?? throw new ArgumentNullException(nameof(periodoSondagemRepository));
        }
        public async Task<PeriodoCompletoSondagemDto> Handle(ObterPeriodoCompletoSondagemPorBimestreQuery request, CancellationToken cancellationToken)
            => await periodoSondagemRepository.ObterPeriodoCompletoPorBimestreEAnoLetivo(request.Bimestre, request.AnoLetivo);
    }
}
