using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterDataPeriodoFimSondagemPorSemestreAnoLetivoQueryHandler : IRequestHandler<ObterDataPeriodoFimSondagemPorSemestreAnoLetivoQuery, DateTime>
    {
        private readonly IPeriodoSondagemRepository periodoSondagemRepository;

        public ObterDataPeriodoFimSondagemPorSemestreAnoLetivoQueryHandler(IPeriodoSondagemRepository periodoSondagemRepository)
        {
            this.periodoSondagemRepository = periodoSondagemRepository ?? throw new ArgumentNullException(nameof(periodoSondagemRepository));
        }
        public async Task<DateTime> Handle(ObterDataPeriodoFimSondagemPorSemestreAnoLetivoQuery request, CancellationToken cancellationToken)
        {
            //TODO Melhorar esta consulta para ser Por Id, mas para isso é necessário alterações no front;
            var descricaoSemestre = $"{request.Semestre}° Semestre";

            var dataFimPeriodoFixo = await periodoSondagemRepository.ObterPeriodoFixoFimPorDescricaoAnoLetivo(descricaoSemestre, request.AnoLetivo);

            if (dataFimPeriodoFixo == default || dataFimPeriodoFixo.Ticks == 0)
            {
                int bimestre;

                if (request.Semestre == 1)
                    bimestre = 2;

                else bimestre = 4;

                var dataFimPeriodoAbertura = await periodoSondagemRepository.ObterPeriodoAberturaFimPorBimestreAnoLetivo(bimestre, request.AnoLetivo);

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
