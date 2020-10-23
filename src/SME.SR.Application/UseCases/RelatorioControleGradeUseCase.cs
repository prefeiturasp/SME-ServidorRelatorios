using MediatR;
using SME.SR.Application.Interfaces;
using SME.SR.Infra;
using System;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class RelatorioControleGradeUseCase : IRelatorioControleGradeUseCase
    {
        private readonly IMediator mediator;

        public RelatorioControleGradeUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Executar(FiltroRelatorioDto request)
        {
            try
            {
                var filtros = request.ObterObjetoFiltro<RelatorioControleGradeFiltroDto>();

                // TODO carregar dados do relatório sintético / analítico
                switch (filtros.Modelo)
                {
                    case ModeloRelatorio.Sintetico:
                        await mediator.Send(new GerarRelatorioControleGradeSinteticoCommand(filtros, request.CodigoCorrelacao));
                        break;
                    case ModeloRelatorio.Analitico:
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
