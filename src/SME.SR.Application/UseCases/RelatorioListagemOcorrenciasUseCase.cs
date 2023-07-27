using MediatR;
using Sentry;
using SME.SR.Application.Interfaces;
using SME.SR.Infra;
using System;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class RelatorioListagemOcorrenciasUseCase : AbstractUseCase, IRelatorioListagemOcorrenciasUseCase
    {
        public RelatorioListagemOcorrenciasUseCase(IMediator mediator) : base(mediator)
        {
        }

        public async Task Executar(FiltroRelatorioDto request)
        {
            try
            {
                var filtros = request.ObterObjetoFiltro<FiltroRelatorioListagemOcorrenciasDto>();
                var relatorioDto = new RelatorioListagemOcorrenciasDto();

                PreencherFiltrosRelatorio(relatorioDto, filtros);
                await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioListagemOcorrencias", relatorioDto, request.CodigoCorrelacao, diretorioComplementar: "ocorrencia"));
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw ex;
            }
        }

        private void PreencherFiltrosRelatorio(RelatorioListagemOcorrenciasDto relatorioDto, FiltroRelatorioListagemOcorrenciasDto filtros)
        {
            relatorioDto.Usuario = $"{filtros.NomeUsuario} ({filtros.CodigoRf})";
            relatorioDto.DataSolicitacao = DateTime.Now;
            //relatorioDto.Dre = filtros.CodigoDre.EstaFiltrandoTodas() ? "TODAS" : relatorioDto.Registros.FirstOrDefault()?.Dre;
            //relatorioDto.Ue = filtros.CodigoUe.EstaFiltrandoTodas() ? "TODAS" : relatorioDto.Registros.FirstOrDefault()?.Ue;
        }
    }
}
