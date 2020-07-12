using MediatR;
using Newtonsoft.Json;
using SME.SR.Application.Queries.RelatorioFaltasFrequencia;
using SME.SR.Infra;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class RelatorioFaltasFrequenciasUseCase : IRelatorioFaltasFrequenciasUseCase
    {
        private readonly IMediator mediator;

        public RelatorioFaltasFrequenciasUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Executar(FiltroRelatorioDto request)
        {

            var relatorioFiltros = request.ObterObjetoFiltro<ObterRelatorioFaltasFrequenciasQuery>();

            var relatorioFaltasFrequencias = await mediator.Send(relatorioFiltros);

            if (relatorioFaltasFrequencias == null)
                throw new NegocioException("Não foi possível localizar dados com os filtros informados.");

            switch (relatorioFiltros.TipoFormatoRelatorio)
            {
                case TipoFormatoRelatorio.Xlsx:
                    var dadosExcel = await mediator.Send(new ObterRelatorioFaltasFrequenciasExcelQuery() { RelatorioFaltasFrequencias = relatorioFaltasFrequencias });
                    if (dadosExcel == null)
                        throw new NegocioException("Não foi possível transformar os dados obtidos em dados excel.");
                    await mediator.Send(new GerarExcelGenericoCommand(dadosExcel.ToList<object>(), "Faltas Frequencias", request.CodigoCorrelacao));
                    break;
                case TipoFormatoRelatorio.Pdf:
                    await GerarRelatorioPdf(mediator, relatorioFiltros);
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

        private async Task GerarRelatorioPdf(IMediator mediator, ObterRelatorioFaltasFrequenciasQuery obterRelatorioFaltasFrequenciasQuery)
        {
            var dadosRelatorio = await mediator.Send(new ObterRelatorioFaltasFrequenciaPdfQuery(obterRelatorioFaltasFrequenciasQuery));
            var dadosJson = JsonConvert.SerializeObject(dadosRelatorio);
            await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioFaltasFrequencias", dadosRelatorio, Guid.NewGuid()));
        }
    }
}
