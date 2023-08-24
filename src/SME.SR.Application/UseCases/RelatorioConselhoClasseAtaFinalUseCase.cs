using MediatR;
using SME.SR.Infra;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class RelatorioConselhoClasseAtaFinalUseCase : IRelatorioConselhoClasseAtaFinalUseCase
    {
        private readonly IMediator mediator;

        public RelatorioConselhoClasseAtaFinalUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Executar(FiltroRelatorioDto request)
        {

            request.RotaErro = RotasRabbitSGP.RotaRelatoriosComErroAtaFinalResultados;
            var filtros = request.ObterObjetoFiltro<FiltroConselhoClasseAtaFinalDto>();
            var mensagensErro = new StringBuilder();
            var relatoriosTurmas = await mediator.Send(new ObterRelatorioConselhoClasseAtaFinalPdfQuery(filtros));

            if (relatoriosTurmas == null || !relatoriosTurmas.Any())
                throw new NegocioException("Não há dados para o relatório de Ata Final de Resultados.");

            switch (filtros.TipoFormatoRelatorio)
            {
                case TipoFormatoRelatorio.Xlsx:
                    var relatorioDto = await mediator.Send(new ObterRelatorioConselhoClasseAtaFinalExcelQuery() { ObjetoExportacao = relatoriosTurmas });
                    if (relatorioDto == null)
                        throw new NegocioException("Não foi possível transformar os dados obtidos em dados excel.");

                    await mediator.Send(new GerarRelatorioAtaFinalExcelCommand(relatorioDto, relatoriosTurmas, "RelatorioAtasComColunaFinal", request.UsuarioLogadoRF));
                    break;
                case TipoFormatoRelatorio.Pdf:
                    await mediator.Send(new GerarRelatorioAtaFinalHtmlParaPdfCommand("RelatorioAtasComColunaFinal", relatoriosTurmas, request.CodigoCorrelacao, mensagensErro.ToString()));
                    break;
                case TipoFormatoRelatorio.Rtf:
                case TipoFormatoRelatorio.Html:
                case TipoFormatoRelatorio.Xls:
                case TipoFormatoRelatorio.Csv:
                case TipoFormatoRelatorio.Xml:
                case TipoFormatoRelatorio.Docx:
                case TipoFormatoRelatorio.Odt:
                case TipoFormatoRelatorio.Ods:
                case TipoFormatoRelatorio.Jrprint:
                default:
                    throw new NegocioException($"Não foi possível exportar este relátorio para o formato {filtros.TipoFormatoRelatorio}");
            }
        }
    }
}
