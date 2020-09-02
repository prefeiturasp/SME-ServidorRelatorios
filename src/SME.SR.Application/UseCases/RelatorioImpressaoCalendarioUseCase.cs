using MediatR;
using SME.SR.Application;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Threading.Tasks;

namespace SME.SR.Workers.SGP
{
    public class RelatorioImpressaoCalendarioUseCase : IRelatorioImpressaoCalendarioUseCase
    {
        private readonly IMediator mediator;

        public RelatorioImpressaoCalendarioUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Executar(FiltroRelatorioDto request)
        {
            var filtros = request.ObterObjetoFiltro<RelatorioImpressaoCalendarioFiltroDto>();

            Dre dre = null;
            if (!string.IsNullOrEmpty(filtros.DreCodigo))
            {
                dre = await mediator.Send(new ObterDrePorCodigoQuery() { DreCodigo = filtros.DreCodigo });
                if (dre == null)
                    throw new NegocioException("Não foi possível localizar a Dre");
            }
            Ue ue = null;
            if (!string.IsNullOrEmpty(filtros.UeCodigo))
            {
                ue = await mediator.Send(new ObterUePorCodigoQuery(filtros.UeCodigo));
                if (ue == null)
                    throw new NegocioException("Não foi possível localizar a Ue");
            }

            var tipoCalendario = await mediator.Send(new ObterTipoCalendarioPorIdQuery(filtros.TipoCalendarioId));
            if (tipoCalendario == null)
                throw new NegocioException("Não foi possível localizar o Tipo de Calendário.");

            var relatorio = await mediator.Send(new ObterRelatorioImpressaoCalendarioQuery(dre, ue, tipoCalendario,
                filtros.EhSME, filtros.UsuarioRF, filtros.UsuarioPerfil, filtros.ConsideraPendenteAprovacao, filtros.PodeVisualizarEventosOcorrenciaDre));

            await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioImpressaoCalendario", relatorio, request.CodigoCorrelacao));
        }
    }
}
