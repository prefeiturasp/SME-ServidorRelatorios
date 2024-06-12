using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application.Queries
{
    public class ObterDataPeriodoFimSondagemPorBimestreAnoLetivoQueryHandler : IRequestHandler<ObterDataPeriodoFimSondagemPorBimestreAnoLetivoQuery, PeriodoCompletoSondagemDto>
    {
        private readonly IPeriodoSondagemRepository periodoSondagemRepository;

        public ObterDataPeriodoFimSondagemPorBimestreAnoLetivoQueryHandler(IPeriodoSondagemRepository periodoSondagemRepository)
        {
            this.periodoSondagemRepository = periodoSondagemRepository ?? throw new ArgumentNullException(nameof(periodoSondagemRepository));
        }
        public async Task<PeriodoCompletoSondagemDto> Handle(ObterDataPeriodoFimSondagemPorBimestreAnoLetivoQuery request, CancellationToken cancellationToken)
        {
            var descricaoBimestre = $"{request.Bimestre}° Bimestre";

            var periodoFixo = await periodoSondagemRepository.ObterPeriodoFixoFimPorDescricaoAnoLetivo(descricaoBimestre, request.AnoLetivo);

            if (periodoFixo is null || periodoFixo.PeriodoFim == default || periodoFixo.PeriodoFim.Ticks == 0)
            {
                var periodoAbertura = await periodoSondagemRepository.ObterPeriodoAberturaFimPorBimestreAnoLetivo(request.Bimestre, request.AnoLetivo);

                if (periodoAbertura is null || periodoAbertura.PeriodoFim == default || periodoAbertura.PeriodoFim.Ticks == 0)
                    throw new NegocioException("Não foi possível localizar a data fim do período da sondagem.");

                return periodoAbertura;
            }

            return periodoFixo;
        }
    }
}
