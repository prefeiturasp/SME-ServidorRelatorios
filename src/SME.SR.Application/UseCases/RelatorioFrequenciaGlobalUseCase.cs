using MediatR;
using SME.SR.Application.Interfaces;
using SME.SR.Data;
using SME.SR.Infra;
using SME.SR.Infra.Utilitarios;
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

        public RelatorioFrequenciaGlobalUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Executar(FiltroRelatorioDto request)
        {
            var filtroRelatorio = request.ObterObjetoFiltro<FiltroFrequenciaGlobalDto>();
            var listaDeFrenquenciaGlobal = await mediator.Send(new ObterRelatorioDeFrequenciaGlobalQuery(filtroRelatorio));
            if (listaDeFrenquenciaGlobal != null && listaDeFrenquenciaGlobal.Any())
            {
                switch (filtroRelatorio.TipoFormatoRelatorio)
                {
                    case TipoFormatoRelatorio.Pdf:
                        await GerarRelatorioPdf(listaDeFrenquenciaGlobal, request, filtroRelatorio);
                        break;
                    case TipoFormatoRelatorio.Xlsx:
                        await ExecuteExcel(listaDeFrenquenciaGlobal, request.CodigoCorrelacao);
                        break;
                    default:
                        throw new NegocioException($"Não foi possível exportar este relátorio para o formato {filtroRelatorio.TipoFormatoRelatorio}");
                }
            }
            else throw new NegocioException("Não foi possível localizar informações com os filtros selecionados");
        }

        private async Task GerarRelatorioPdf(List<FrequenciaGlobalDto> listaDeFrequencia, FiltroRelatorioDto request, FiltroFrequenciaGlobalDto filtroRelatorio)
        {
            var dto = new FrequenciaMensalDto();
            await MapearCabecalho(dto, filtroRelatorio);
        }

        private async Task MapearCabecalho(FrequenciaMensalDto dto, FiltroFrequenciaGlobalDto filtroRelatorio)
        {
            //Se Selecionar todas ues as turmas por padrão são todas, Só informa turma se todas uma UE expecifica
            //var dadosTurma = await ObterTurma(filtroRelatorio.CodigosTurmas.FirstOrDefault());
            //var dadosDreUe = await ObterNomeDreUe(dadosTurma.Codigo);
            //dto.Cabecalho.NomeDre = dadosDreUe?.DreNome;
            //dto.Cabecalho.NomeUe = dadosDreUe?.UeNome;
            //dto.Cabecalho.AnoLetivo = filtroRelatorio.AnoLetivo;
            //dto.Cabecalho.NomeModalidade = filtroRelatorio.Modalidade.ShortName();
            //dto.Cabecalho.MesReferencia = filtroRelatorio.MesesReferencias
        }
        private async Task<Turma> ObterTurma(string codigoTurma)
        {
            return await mediator.Send(new ObterTurmaPorCodigoQuery(codigoTurma));
        }
        private async Task<DreUe> ObterNomeDreUe(string turmaCodigo)
        {
            return await mediator.Send(new ObterDreUePorTurmaCodigoQuery(turmaCodigo));
        }
        private async Task ExecuteExcel(List<FrequenciaGlobalDto> listaDeFrequencia, Guid codigoCorrelacao)
        {
            await mediator.Send(new GerarExcelGenericoCommand(listaDeFrequencia.Cast<object>().ToList(), "Frequência Global", codigoCorrelacao));
        }
    }
}
