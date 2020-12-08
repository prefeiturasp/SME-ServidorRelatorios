using MediatR;
using SME.SR.Application.Interfaces;
using SME.SR.Infra;
using System;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class RelatorioLeituraComunicadosUseCase : IRelatorioLeituraComunicadosUseCase
    {
        private readonly IMediator mediator;

        public RelatorioLeituraComunicadosUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Executar(FiltroRelatorioDto request)
        {
            try
            {
                var filtro = request.ObterObjetoFiltro<FiltroRelatorioLeituraComunicadosDto>();
                var relatorioDto = new RelatorioLeituraComunicadosDto();

                await ObterFiltroRelatorio(relatorioDto, filtro, request.UsuarioLogadoRF);
                await ObterDadosRelatorio(relatorioDto, filtro);

                await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioLeituraComunicados", relatorioDto, request.CodigoCorrelacao));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private async Task ObterDadosRelatorio(RelatorioLeituraComunicadosDto relatorioDto, FiltroRelatorioLeituraComunicadosDto filtro)
        {
            throw new NotImplementedException();
        }

        private async Task ObterFiltroRelatorio(RelatorioLeituraComunicadosDto relatorioDto, FiltroRelatorioLeituraComunicadosDto filtro, string usuarioLogadoRF)
        {
            var filtroRelatorio = new FiltroLeituraComunicadosDto();


            filtroRelatorio.Dre = await ObterNomeDre(filtro.CodigoDre);
            filtroRelatorio.Ue = await ObterNomeUe(filtro.CodigoUe);
            filtroRelatorio.Usuario = filtro.NomeUsuario;
            filtroRelatorio.RF = usuarioLogadoRF;

            relatorioDto.Filtro = filtroRelatorio;
        }

        private async Task<string> ObterNomeDre(string dreCodigo)
        {
            var dre = dreCodigo.Equals("-99") ? null :
                await mediator.Send(new ObterDrePorCodigoQuery(dreCodigo));

            return dre != null ? dre.Abreviacao : "Todas";
        }

        private async Task<string> ObterNomeUe(string ueCodigo)
        {
            var ue = ueCodigo.Equals("-99") ? null :
                await mediator.Send(new ObterUePorCodigoQuery(ueCodigo));

            return ue != null ? ue.NomeRelatorio : "Todas";
        }
    }
}
