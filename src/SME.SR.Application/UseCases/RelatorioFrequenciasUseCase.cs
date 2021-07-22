using MediatR;
using SME.SR.Application.Queries.RelatorioFaltasFrequencia;
using SME.SR.Infra;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class RelatorioFrequenciasUseCase : IRelatorioFrequenciasUseCase
    {
        private readonly IMediator mediator;

        public RelatorioFrequenciasUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Executar(FiltroRelatorioDto request)
        {
            var relatorioFiltros = request.ObterObjetoFiltro<FiltroRelatorioFrequenciasDto>();           

            var dadosRelatorio = await mediator.Send(new ObterRelatorioFrequenciaPdfQuery(relatorioFiltros));

            switch (relatorioFiltros.TipoFormatoRelatorio)
            {
                case TipoFormatoRelatorio.Xlsx:
                    var relatorioDto = await mediator.Send(new ObterRelatorioFaltasFrequenciasExcelQuery() { RelatorioFaltasFrequencias = dadosRelatorio, TipoRelatorio = relatorioFiltros.TipoRelatorio });
                    if (relatorioDto == null)
                        throw new NegocioException("Não foi possível transformar os dados obtidos em dados excel.");

                    switch (relatorioFiltros.TipoRelatorio)
                    {
                        case TipoRelatorioFaltasFrequencia.Ano:
                            var relatorioAno = relatorioDto.OfType<RelatorioFaltasFrequenciasExcelDto>();
                            await mediator.Send(new GerarExcelGenericoCommand(relatorioAno.ToList<object>(), "Faltas Frequencias", request.CodigoCorrelacao));
                            break;
                        case TipoRelatorioFaltasFrequencia.Turma:
                            var relatorioTurma = relatorioDto.OfType<RelatorioFaltasExcelDto>();
                            await mediator.Send(new GerarExcelGenericoCommand(relatorioTurma.ToList<object>(), "Faltas Frequencias", request.CodigoCorrelacao));
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
