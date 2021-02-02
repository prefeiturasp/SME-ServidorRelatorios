using MediatR;
using SME.SR.Application.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class RelatorioUsuariosUseCase : IRelatorioUsuariosUseCase
    {
        private readonly IMediator mediator;

        public RelatorioUsuariosUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Executar(FiltroRelatorioDto request)
        {
            try
            {
                var filtro = request.ObterObjetoFiltro<FiltroRelatorioUsuariosDto>();
                var relatorioDto = new RelatorioUsuarioDto();

                await ObterFiltroRelatorio(relatorioDto, filtro, request.UsuarioLogadoRF);
                await ObterDadosRelatorioUsuarios(relatorioDto, filtro);

                await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioUsuarios", relatorioDto, request.CodigoCorrelacao));
            }
            catch (Exception ex)
            {
                throw;
            }        
        }

        private async Task ObterDadosRelatorioUsuarios(RelatorioUsuarioDto relatorioDto, FiltroRelatorioUsuariosDto filtro)
        {
            relatorioDto.DadosRelatorio = await mediator.Send(new ObterDadosRelatorioUsuariosCommand(filtro));
        }

        private async Task ObterFiltroRelatorio(RelatorioUsuarioDto relatorioDto, FiltroRelatorioUsuariosDto filtro, string usuarioLogadoRF)
        {
            var filtroRelatorio = new FiltroUsuarioDto();

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
