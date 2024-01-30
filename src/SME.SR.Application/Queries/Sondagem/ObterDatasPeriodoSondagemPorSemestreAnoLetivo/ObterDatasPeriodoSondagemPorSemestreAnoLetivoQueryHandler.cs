using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterDatasPeriodoSondagemPorSemestreAnoLetivoQueryHandler : IRequestHandler<ObterDatasPeriodoSondagemPorSemestreAnoLetivoQuery, PeriodoCompletoSondagemDto>
    {
        private readonly IPeriodoSondagemRepository periodoSondagemRepository;

        public ObterDatasPeriodoSondagemPorSemestreAnoLetivoQueryHandler(IPeriodoSondagemRepository periodoSondagemRepository)
        {
            this.periodoSondagemRepository = periodoSondagemRepository ?? throw new ArgumentNullException(nameof(periodoSondagemRepository));
        }
        public async Task<PeriodoCompletoSondagemDto> Handle(ObterDatasPeriodoSondagemPorSemestreAnoLetivoQuery request, CancellationToken cancellationToken)
        {
            //TODO Melhorar esta consulta para ser Por Id, mas para isso é necessário alterações no front;
            var descricaoSemestre = $"{request.Semestre}%Semestre";

            var periodoFixo = await periodoSondagemRepository.ObterPeriodoFixoCompletoPorDescricaoEAnoLetivo(descricaoSemestre, request.AnoLetivo);

            if (periodoFixo is null || periodoFixo.PeriodoFim.Ticks == 0)
            {
                int bimestre;

                if (request.Semestre == 1)
                    bimestre = 2;

                else bimestre = 4;

                var periodoAbertura = await periodoSondagemRepository.ObterPeriodoCompletoPorBimestreEAnoLetivo(bimestre, request.AnoLetivo.ToString());

                if (periodoAbertura is null || periodoAbertura.PeriodoFim.Ticks == 0)
                    throw new NegocioException("Não foi possível localizar a data fim do período da sondagem.");

                return periodoAbertura;
            }
            else
                return periodoFixo;
        }
    }
}
