using MediatR;
using SME.SR.Application.Interfaces;
using SME.SR.Infra;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class RelatorioAnaliticoSondagemUseCase : IRelatorioAnaliticoSondagemUseCase
    {
        private readonly IMediator mediator;

        public RelatorioAnaliticoSondagemUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Executar(FiltroRelatorioDto request)
        {
            //var filtro = request.ObterObjetoFiltro<FiltroRelatorioAnaliticoSondagemDto>();
            var filtro = new FiltroRelatorioAnaliticoSondagemDto
            {
                AnoLetivo= 2021,
                AnoTurma= "4",
                Semestre= 4,
                Bimestre= 4,
                DreCodigo= "108300",
                UeCodigo= "019406",
                TipoSondagem= TipoSondagem.LP_ProducaoTexto
            };
            request.CodigoCorrelacao = Guid.NewGuid();
            var relatorios = await mediator.Send(new ObterRelatorioAnaliticoSondagemQuery(filtro));
    
            if (relatorios == null || !relatorios.Any())
                throw new NegocioException("Não há dados para o relatório analítico da sondagem.");

            await mediator.Send(new GerarRelatorioAnaliticoDaSondagemExcelCommand(relatorios, filtro.TipoSondagem, request.CodigoCorrelacao));
        }
    }
}
