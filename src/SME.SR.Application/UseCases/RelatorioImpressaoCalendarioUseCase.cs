using MediatR;
using SME.SR.Application;
using SME.SR.Infra;
using System;
using System.Threading.Tasks;

namespace SME.SR.Workers.SGP
{
    public class RelatorioImpressaoCalendarioUseCase : IRelatorioBoletimEscolarUseCase
    {
        private readonly IMediator mediator;

        public RelatorioImpressaoCalendarioUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Executar(FiltroRelatorioDto request)
        {
            var relatorioQuery = request.ObterObjetoFiltro<RelatorioImpressaoCalendarioFiltroDto>();
            var relatorio = await mediator.Send(new ObterRelatorioImpressaoCalendarioQuery(relatorioQuery.DreCodigo, relatorioQuery.UeCodigo, relatorioQuery.TipoCalendarioId, relatorioQuery.EhSME, relatorioQuery.UsuarioRF, relatorioQuery.UsuarioPerfil));

            await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioImpressaoCalendario", relatorio, request.CodigoCorrelacao));
        }
    }
}
