using MediatR;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static SME.SR.Infra.Enumeradores;

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
            try
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
                        throw new NegocioException($"Não foi possível exportar este relátorio para o formato {relatorioFiltros.TipoFormatoRelatorio.ToString()}");
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
