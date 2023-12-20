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
        private int contador;
        private Guid sessionId;

        public RelatorioConselhoClasseAtaFinalUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            contador = 0;
            sessionId = Guid.NewGuid();
        }

        public async Task Executar(FiltroRelatorioDto request)
        {
            request.RotaErro = RotasRabbitSGP.RotaRelatoriosComErroAtaFinalResultados;
            var filtros = request.ObterObjetoFiltro<FiltroConselhoClasseAtaFinalDto>();
            var mensagensErro = new StringBuilder();

            await Logar("Inicio Processo");
            var relatoriosTurmas = await mediator.Send(new ObterRelatorioConselhoClasseAtaFinalPdfQuery(filtros));
            await Logar("Dados Obtidos");

            if (relatoriosTurmas == null || !relatoriosTurmas.Any())
                throw new NegocioException("Não há dados para o relatório de Ata Final de Resultados.");

            switch (filtros.TipoFormatoRelatorio)
            {
                case TipoFormatoRelatorio.Xlsx:
                    await Logar("Montando Excel");
                    var relatorioDto = await mediator.Send(new ObterRelatorioConselhoClasseAtaFinalExcelQuery() { ObjetoExportacao = relatoriosTurmas });
                    await Logar("Excel Montado");
                    if (relatorioDto == null)
                        throw new NegocioException("Não foi possível transformar os dados obtidos em dados excel.");

                    await Logar("Gerando Relatório Excel");
                    await mediator.Send(new GerarRelatorioAtaFinalExcelCommand(relatorioDto, relatoriosTurmas, "RelatorioAtasComColunaFinal", request.UsuarioLogadoRF));
                    await Logar("Relatório Excel Gerado");
                    break;
                case TipoFormatoRelatorio.Pdf:
                    await Logar("Gerando Relatório PDF");
                    await mediator.Send(new GerarRelatorioAtaFinalHtmlParaPdfCommand("RelatorioAtasComColunaFinal", relatoriosTurmas, request.CodigoCorrelacao, mensagensErro.ToString(), sessionId));
                    await Logar("Relatório PDF Gerado");
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

        private Task Logar(string mensagem)
            => mediator.Send(new SalvarLogViaRabbitCommand($"{sessionId}:{++contador}: {mensagem}", LogNivel.Informacao));
    }
}
