using MediatR;
using SME.SR.Application.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class RelatorioAlteracaoNotasUseCase : IRelatorioAlteracaoNotasUseCase
    {
        private readonly IMediator mediator;

        public RelatorioAlteracaoNotasUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Executar(FiltroRelatorioDto request)
        {
            try
            {
                var filtro = request.ObterObjetoFiltro<FiltroRelatorioAlteracaoNotasDto>();
                var relatorioDto = new RelatorioAlteracaoNotasDto();

                await mediator.Send(new GerarRelatorioAlteracaoNotasCommand(filtro, request.CodigoCorrelacao));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private async Task ObterDadosRelatorio(RelatorioAlteracaoNotasDto relatorioDto, FiltroRelatorioUsuariosDto filtro)
        {
            relatorioDto.Turmas = await mediator.Send(new ObterDadosRelatorioUsuariosCommand(filtro));
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

            return dre != null ? dre.Nome : "Todas";
        }

        private async Task<string> ObterNomeUe(string ueCodigo)
        {
            var ue = ueCodigo.Equals("-99") ? null :
                await mediator.Send(new ObterUePorCodigoQuery(ueCodigo));

            return ue != null ? ue.NomeRelatorio : "Todas";
        }
    }
}
