using MediatR;
using SME.SR.Application.Interfaces;
using SME.SR.Infra;
using System;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class RelatorioRegistroIndividualUseCase : IRelatorioRegistroIndividualUseCase
    {
        private readonly IMediator mediator;

        public RelatorioRegistroIndividualUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Executar(FiltroRelatorioDto filtro)
        {
            var parametros = filtro.ObterObjetoFiltro<FiltroRelatorioRegistroIndividualDto>();

            var turma = await mediator.Send(new ObterTurmaPorIdQuery(parametros.TurmaId));

            try
            {
                var relatorioDto = new RelatorioRegistroIndividualDto();

                relatorioDto = await mediator.Send(new ObterDadosConsolidadosRegistroIndividualParaRelatorioQuery());

                await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioRegistroIndividual", relatorioDto, filtro.CodigoCorrelacao, gerarPaginacao: false));

            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
    }
}
