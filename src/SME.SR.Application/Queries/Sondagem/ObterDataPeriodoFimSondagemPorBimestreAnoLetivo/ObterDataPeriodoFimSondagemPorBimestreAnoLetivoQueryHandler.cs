using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application.Queries
{
    public class ObterDataPeriodoFimSondagemPorBimestreAnoLetivoQueryHandler : IRequestHandler<ObterDataPeriodoFimSondagemPorBimestreAnoLetivoQuery, DateTime>
    {
        private readonly IPeriodoSondagemRepository periodoSondagemRepository;

        public ObterDataPeriodoFimSondagemPorBimestreAnoLetivoQueryHandler(IPeriodoSondagemRepository periodoSondagemRepository)
        {
            this.periodoSondagemRepository = periodoSondagemRepository ?? throw new ArgumentNullException(nameof(periodoSondagemRepository));
        }
        public async Task<DateTime> Handle(ObterDataPeriodoFimSondagemPorBimestreAnoLetivoQuery request, CancellationToken cancellationToken)
        {
            //TODO: Melhorar esta consulta para ser Por Id, mas para isso é necessário alterações no front;
            var descricaoBimestre = $"{request.Bimestre}° Bimestre";

            var dataFimPeriodoFixo = await periodoSondagemRepository.ObterPeriodoFixoFimPorDescricaoAnoLetivo(descricaoBimestre, request.AnoLetivo);

            if (dataFimPeriodoFixo == default || dataFimPeriodoFixo.Ticks == 0)
            {
                var dataFimPeriodoAbertura = await periodoSondagemRepository.ObterPeriodoAberturaFimPorBimestreAnoLetivo(request.Bimestre, request.AnoLetivo);

                if (dataFimPeriodoAbertura == default || dataFimPeriodoAbertura.Ticks == 0)
                    throw new NegocioException("Não foi possível localizar a data fim do período da sondagem.");

                return dataFimPeriodoAbertura;
            }
            else
            {
                return dataFimPeriodoFixo;
            }
        }
    }
}
