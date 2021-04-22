using MediatR;
using SME.SR.Application.Interfaces;
using SME.SR.Infra;
using System;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class RelatorioAcompanhamentoAprendizagemUseCase : IRelatorioAcompanhamentoAprendizagemUseCase
    {
        private readonly IMediator mediator;

        public RelatorioAcompanhamentoAprendizagemUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Executar(FiltroRelatorioDto filtro)
        {
            var parametros = filtro.ObterObjetoFiltro<FiltroRelatorioAcompanhamentoAprendizagemDto>();

            try
            {
                var relatorioDto = new RelatorioAcompanhamentoAprendizagemDto();

                relatorioDto = await mediator.Send(new ObterRelatorioAcompanhamentoAprendizagemQuery(parametros.TurmaCodigo, parametros.AlunoCodigo));

                await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioAcompanhamentoAprendizagem", relatorioDto, filtro.CodigoCorrelacao));

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
