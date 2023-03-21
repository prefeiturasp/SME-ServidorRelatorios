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
            //var filtros = request.ObterObjetoFiltro<FiltroRelatorioAnaliticoSondagemDto>();
            var filtros = new FiltroRelatorioAnaliticoSondagemDto
            {
                AnoLetivo= 2021,
                AnoTurma= "3",
                Semestre= 1,
                Bimestre= 1,
                DreCodigo= "108300",
                UeCodigo= "019406",
                TipoSondagem= TipoSondagem.LP_Leitura
            };
            var relatorios = await mediator.Send(new ObterRelatorioAnaliticoSondagemQuery(filtros));

            if (relatorios == null || !relatorios.Any())
                throw new NegocioException("Não há dados para o relatório analítico da sondagem.");

            //await mediator.Send(new GerarRelatorioAtaFinalExcelCommand(relatorioDto, relatoriosTurmas, "RelatorioAtasComColunaFinal", request.UsuarioLogadoRF));
        }
    }
}
