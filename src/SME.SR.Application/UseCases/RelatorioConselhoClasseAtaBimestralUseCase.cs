using MediatR;
using SME.SR.Infra;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class RelatorioConselhoClasseAtaBimestralUseCase : IRelatorioConselhoClasseAtaBimestralUseCase
    {
        private readonly IMediator mediator;

        public RelatorioConselhoClasseAtaBimestralUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Executar(FiltroRelatorioDto request)
        {

            var filtros = request.ObterObjetoFiltro<FiltroConselhoClasseAtaBimestralDto>();
            var mensagensErro = new StringBuilder();
            var relatoriosTurmas = await mediator.Send(new ObterRelatorioConselhoClasseAtaBimestralPdfQuery(filtros));

              if (!relatoriosTurmas.Any())
                throw new NegocioException("Não há dados para o relatório de Ata Bimestral de Resultados.");


            await mediator.Send(new GerarRelatorioAtaBimestralHtmlParaPdfCommand("RelatorioAtaBimestralComColunaFinal.html", relatoriosTurmas, request.CodigoCorrelacao, mensagensErro.ToString()));

        }
    }
}

