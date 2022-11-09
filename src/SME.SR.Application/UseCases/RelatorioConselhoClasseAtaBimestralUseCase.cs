using MediatR;
using SME.SR.Infra;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class RelatorioConselhoClasseAtaBimestralUseCase : AbstractUseCase, IRelatorioConselhoClasseAtaBimestralUseCase
    {
        public RelatorioConselhoClasseAtaBimestralUseCase(IMediator mediator) : base(mediator)
        {
        }
        public async Task Executar(FiltroRelatorioDto request)
        {
            var filtros = request.ObterObjetoFiltro<FiltroConselhoClasseAtaBimestralDto>();
            var mensagensErro = new StringBuilder();
            var relatoriosTurmas = await mediator.Send(new ObterRelatorioConselhoClasseAtaBimestralPdfQuery(filtros));

            if (relatoriosTurmas == null || !relatoriosTurmas.Any())
                throw new NegocioException("Não há dados para o relatório de Ata Bimestral de Resultados.");

            await mediator.Send(new GerarRelatorioAtaBimestralHtmlParaPdfCommand("RelatorioAtaBimestralComColunaFinal.html", relatoriosTurmas, request.CodigoCorrelacao, mensagensErro.ToString()));
        }
    }
}

