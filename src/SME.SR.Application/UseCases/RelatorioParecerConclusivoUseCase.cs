using MediatR;
using Newtonsoft.Json;
using SME.SR.Infra;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class RelatorioParecerConclusivoUseCase : IRelatorioParecerConclusivoUseCase
    {
        private readonly IMediator mediator;

        public RelatorioParecerConclusivoUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Executar(FiltroRelatorioDto request)
        {
            request.RotaErro = RotasRabbit.RotaRelatoriosComErroParecerConclusivo;
            var relatorioFiltros = request.ObterObjetoFiltro<FiltroRelatorioParecerConclusivoDto>();

            var resultado = await mediator.Send(new ObterRelatorioParecerConclusivoQuery() { filtroRelatorioParecerConclusivoDto = relatorioFiltros, UsuarioRf = request.UsuarioLogadoRF });

            switch (relatorioFiltros.TipoFormatoRelatorio)
            {
                case TipoFormatoRelatorio.Xlsx:
                    var dadosExcel = await mediator.Send(new ObterRelatorioParecerConclusivoExcelQuery() { RelatorioParecerConclusivo = resultado });
                    if (dadosExcel == null)
                        throw new NegocioException("Não foi possível transformar os dados obtidos em dados excel.");
                    await mediator.Send(new GerarExcelGenericoCommand(dadosExcel.ToList<object>(), "RelatorioParecerConclusivo", request.CodigoCorrelacao));
                    break;
                case TipoFormatoRelatorio.Pdf:
                    await GerarRelatorioPdf(mediator, resultado, request.CodigoCorrelacao);
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
                    throw new NegocioException($"Não foi possível exportar este relátorio para o formato {relatorioFiltros.TipoFormatoRelatorio}");
            }
        }

        private async Task GerarRelatorioPdf(IMediator mediator, RelatorioParecerConclusivoDto dadosRelatorio, Guid codigoCorrelacao)
        {
            await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioParecerConclusivo", dadosRelatorio, codigoCorrelacao));
        }

    }
}
