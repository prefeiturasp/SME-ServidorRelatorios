using MediatR;
using SME.SR.Application.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class RelatorioFrequenciaGlobalUseCase : IRelatorioFrequenciaGlobalUseCase
    {
        public delegate Task OpcaoRelatorio(List<FrequenciaGlobalDto> listaDeFrequencia, Guid codigoCorrelacao);

        public readonly IMediator mediator;

        private FiltroFrequenciaGlobalDto FitroRelatorio { get; set; }

        public RelatorioFrequenciaGlobalUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Executar(FiltroRelatorioDto request)
        {
            var dicionarioDeOpcoesRelatorio = ObtenhaDicionarioDeOpcoesRelatorio();
            FitroRelatorio = request.ObterObjetoFiltro<FiltroFrequenciaGlobalDto>();
            var listaDeFrenquenciaGlobal = await mediator.Send(new ObterRelatorioDeFrequenciaGlobalQuery(FitroRelatorio));

            if (dicionarioDeOpcoesRelatorio.ContainsKey(FitroRelatorio.TipoFormatoRelatorio))
            {
                await dicionarioDeOpcoesRelatorio[FitroRelatorio.TipoFormatoRelatorio].Invoke(listaDeFrenquenciaGlobal, request.CodigoCorrelacao);
            }
            else
            {
                throw new NegocioException($"Não foi possível exportar este relátorio para o formato {FitroRelatorio.TipoFormatoRelatorio}");
            }
        }

        private Dictionary<TipoFormatoRelatorio, OpcaoRelatorio> ObtenhaDicionarioDeOpcoesRelatorio()
        {
            var dicionario = new Dictionary<TipoFormatoRelatorio, OpcaoRelatorio>
            {
                { TipoFormatoRelatorio.Xlsx, ExecuteExcel }
            };

            return dicionario;
        }

        private async Task ExecuteExcel(List<FrequenciaGlobalDto> listaDeFrequencia, Guid codigoCorrelacao)
        {
            await mediator.Send(new GerarExcelGenericoCommand(listaDeFrequencia.Cast<object>().ToList(), "Frequência Global", codigoCorrelacao));
        }
    }
}
