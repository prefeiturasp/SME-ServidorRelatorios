using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class RelatorioSondagemComponentesPorTurmaUseCase : IRelatorioSondagemComponentesPorTurmaUseCase
    {
        private readonly IMediator mediator;

        public RelatorioSondagemComponentesPorTurmaUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        public async Task Executar(RelatorioSondagemComponentesPorTurmaFiltroDto request)
        {
            var filtros = request.ObterObjetoFiltro<RelatorioSondagemComponentesPorTurmaFiltroDto>();

            var relatorio = await ObterDadosRelatorio(filtros);

            if (relatorio == null)
                throw new NegocioException("Não foi possível localizar dados com os filtros informados.");

            await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioSondagemComponentesPorTurma", relatorio, request.CodigoCorrelacao));
        }

        private async Task<RelatorioSondagemComponentesPorTurmaRelatorioDto> ObterDadosRelatorio(RelatorioSondagemComponentesPorTurmaFiltroDto filtros)
        {
            return await mediator.Send(
               new ObterRelatorioSondagemComponentesPorTurmaQuery()
               {
                    Ano = filtros.Ano,
                    DreId = filtros.DreId,
                    TurmaId = filtros.TurmaId,
                    UeId = filtros.UeId
               });
        }
    }
}
