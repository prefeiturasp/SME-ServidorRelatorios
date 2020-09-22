using MediatR;
using SME.SR.Infra;
using SME.SR.Infra.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class RelatorioFechamentoPendenciasUseCase : IRelatorioFechamentoPendenciasUseCase
    {
        private readonly IMediator mediator;

        public RelatorioFechamentoPendenciasUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Executar(FiltroRelatorioDto request)
        {
            var filtros = request.ObterObjetoFiltro<FiltroRelatorioPendenciasFechamentoDto>();

            var resultado = await mediator.Send(new ObterRelatorioFechamentoPedenciasQuery() { filtroRelatorioPendenciasFechamentoDto = filtros });
            
            await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioFechamentoPendencias", resultado, request.CodigoCorrelacao));

        }     

    }
}
