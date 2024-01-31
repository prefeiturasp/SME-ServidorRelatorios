using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application.Queries
{
    public class ObterDatasPeriodoSondagemPorBimestreAnoLetivoQueryHandler : IRequestHandler<ObterDatasPeriodoSondagemPorBimestreAnoLetivoQuery, PeriodoCompletoSondagemDto>
    {
        private readonly IPeriodoSondagemRepository periodoSondagemRepository;

        public ObterDatasPeriodoSondagemPorBimestreAnoLetivoQueryHandler(IPeriodoSondagemRepository periodoSondagemRepository)
        {
            this.periodoSondagemRepository = periodoSondagemRepository ?? throw new ArgumentNullException(nameof(periodoSondagemRepository));
        }
        public async Task<PeriodoCompletoSondagemDto> Handle(ObterDatasPeriodoSondagemPorBimestreAnoLetivoQuery request, CancellationToken cancellationToken)
        {

            var descricaoBimestre = $"{request.Bimestre}%Bimestre";

            var periodoFixo = await periodoSondagemRepository.ObterPeriodoFixoCompletoPorDescricaoEAnoLetivo(descricaoBimestre, request.AnoLetivo);

            if (periodoFixo is null || periodoFixo.PeriodoFim.Ticks == 0)
            {
                var periodoAbertura = await periodoSondagemRepository.ObterPeriodoCompletoPorBimestreEAnoLetivo(request.Bimestre, request.AnoLetivo);

                if (periodoAbertura is null || periodoAbertura.PeriodoFim.Ticks == 0)
                    throw new NegocioException("Não foi possível localizar a data fim do período da sondagem.");

                return periodoAbertura;
            }
            else
                return periodoFixo;
        }
    }
}
