using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterDataPeriodoFimSondagemPorSemestreAnoLetivoQueryHandler : IRequestHandler<ObterDataPeriodoFimSondagemPorSemestreAnoLetivoQuery, PeriodoCompletoSondagemDto>
    {
        private readonly IPeriodoSondagemRepository periodoSondagemRepository;

        public ObterDataPeriodoFimSondagemPorSemestreAnoLetivoQueryHandler(IPeriodoSondagemRepository periodoSondagemRepository)
        {
            this.periodoSondagemRepository = periodoSondagemRepository ?? throw new ArgumentNullException(nameof(periodoSondagemRepository));
        }
        public async Task<PeriodoCompletoSondagemDto> Handle(ObterDataPeriodoFimSondagemPorSemestreAnoLetivoQuery request, CancellationToken cancellationToken)
        {
            //TODO Melhorar esta consulta para ser Por Id, mas para isso é necessário alterações no front;
            var descricaoSemestre = $"{request.Semestre}° Semestre";

            var periodoFixo = await periodoSondagemRepository.ObterPeriodoFixoFimPorDescricaoAnoLetivo(descricaoSemestre, request.AnoLetivo);

            if (periodoFixo is null || periodoFixo.PeriodoFim  == default || periodoFixo.PeriodoFim.Ticks == 0)
            {
                int bimestre;

                if (request.Semestre == 1)
                    bimestre = 2;

                else bimestre = 4;

                var periodoAbertura = await periodoSondagemRepository.ObterPeriodoAberturaFimPorBimestreAnoLetivo(bimestre, request.AnoLetivo);

                if (periodoAbertura is null || periodoAbertura.PeriodoFim == default || periodoAbertura.PeriodoFim.Ticks == 0)
                    throw new NegocioException("Não foi possível localizar a data fim do período da sondagem.");

                return periodoAbertura;
            }

            return periodoFixo;
        }
    }
}
