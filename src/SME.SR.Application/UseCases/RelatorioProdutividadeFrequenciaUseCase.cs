using MediatR;
using SME.SR.Application.Interfaces;
using SME.SR.Infra;
using SME.SR.Infra.Utilitarios;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class RelatorioProdutividadeFrequenciaUseCase : IRelatorioProdutividadeFrequenciaUseCase
    {
        private readonly IMediator mediator;

        public RelatorioProdutividadeFrequenciaUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Executar(FiltroRelatorioDto request)
        {
            var filtroRelatorio = request.ObterObjetoFiltro<FiltroRelatorioProdutividadeFrequenciaDto>();
            var consolidacoesProdutividadeFrequencia = await mediator.Send(new ObterObterProdutividadeFrequenciaPorFiltroQuery(filtroRelatorio));

            if (consolidacoesProdutividadeFrequencia == null || !consolidacoesProdutividadeFrequencia.Any())
                throw new NegocioException("Nenhuma informação para os filtros informados.");


            await mediator.Send(new GerarRelatoricoProdutividadeFrequenciaExcelCommand(consolidacoesProdutividadeFrequencia, filtroRelatorio, request.CodigoCorrelacao));
        }
    }
}
