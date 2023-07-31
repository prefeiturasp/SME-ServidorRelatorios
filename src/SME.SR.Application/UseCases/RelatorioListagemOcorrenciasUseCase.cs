using MediatR;
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
            var filtros = request.ObterObjetoFiltro<FiltroRelatorioListagemOcorrenciasDto>();
            var relatorioDto = new RelatorioListagemOcorrenciasDto();

            PreencherFiltrosRelatorio(relatorioDto, filtros);
            await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioListagemOcorrencias", relatorioDto, request.CodigoCorrelacao, diretorioComplementar: "ocorrencia"));
        }

        private void PreencherFiltrosRelatorio(RelatorioListagemOcorrenciasDto relatorioDto, FiltroRelatorioListagemOcorrenciasDto filtros)
        {
            relatorioDto.Usuario = $"{filtros.NomeUsuario} ({filtros.CodigoRf})";
            relatorioDto.DataSolicitacao = DateTime.Now;
        }
    }
}
