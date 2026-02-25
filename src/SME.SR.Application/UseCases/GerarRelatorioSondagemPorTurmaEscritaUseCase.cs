using MediatR;
using SME.SR.Application.Commands.Sondagem.EscritaTurma;
using SME.SR.Application.Interfaces.UseCases;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Application.UseCases
{
    public class GerarRelatorioSondagemPorTurmaEscritaUseCase : IGerarRelatorioSondagemPorTurmaEscritaUseCase
    {
        private readonly IMediator mediator;

        public GerarRelatorioSondagemPorTurmaEscritaUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        public async Task Executar(FiltroRelatorioDto request)
        {
            await mediator.Send(new GerarRelatorioSondagemPorTurmaEscritaCommand(
                    Guid.NewGuid(),
                    3019576,
                    1,
                    1,
                    Modalidade.Fundamental,
                    1,
                    2026,
                    0,
                    "Thiago",
                    "400398",
                    1
                    ));
        }
    }
}
