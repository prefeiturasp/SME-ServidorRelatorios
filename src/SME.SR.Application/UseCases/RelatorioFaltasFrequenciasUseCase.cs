using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SME.SR.Application.Queries.RelatorioFaltasFrequencia;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
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
            var relatorioFiltros = request.ObterObjetoFiltro<FiltroRelatorioFaltasFrequenciasDto>();           

            var dadosRelatorio = await mediator.Send(new ObterRelatorioFaltasFrequenciaPdfQuery(relatorioFiltros));

            switch (relatorioFiltros.TipoFormatoRelatorio)
            {
                case TipoFormatoRelatorio.Xlsx:
                    var relatorioDto = await mediator.Send(new ObterRelatorioFaltasFrequenciasExcelQuery() { RelatorioFaltasFrequencias = dadosRelatorio, TipoRelatorio = relatorioFiltros.TipoRelatorio });
                    if (relatorioDto == null)
                        throw new NegocioException("Não foi possível transformar os dados obtidos em dados excel.");

                    switch (relatorioFiltros.TipoRelatorio)
                    {
                        case TipoRelatorioFaltasFrequencia.Ambos:
                            var relatorioAmbos = relatorioDto.OfType<RelatorioFaltasFrequenciasExcelDto>();
                            await mediator.Send(new GerarExcelGenericoCommand(relatorioAmbos.ToList<object>(), "Faltas Frequencias", request.CodigoCorrelacao));
                            break;
                        case TipoRelatorioFaltasFrequencia.Faltas:
                            var relatorioFaltas = relatorioDto.OfType<RelatorioFaltasExcelDto>();
                            await mediator.Send(new GerarExcelGenericoCommand(relatorioFaltas.ToList<object>(), "Faltas Frequencias", request.CodigoCorrelacao));
                            break;
                        case TipoRelatorioFaltasFrequencia.Frequencia:
                            var relatorioFrequencias = relatorioDto.OfType<RelatorioFrequenciasExcelDto>();
                            await mediator.Send(new GerarExcelGenericoCommand(relatorioFrequencias.ToList<object>(), "Faltas Frequencias", request.CodigoCorrelacao));
                            break;
                        default:
                            throw new NegocioException($"Não foi possível exportar este relátorio para o tipo {relatorioFiltros.TipoRelatorio}");
                    }
                    break;
                case TipoFormatoRelatorio.Pdf:
                    await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioFaltasFrequencias", dadosRelatorio, request.CodigoCorrelacao));
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


    }
}
