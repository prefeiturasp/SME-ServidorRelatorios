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
            request.RotaErro = RotasRabbit.RotaRelatoriosComErroEscolaAquiLeitura;
            try
            {
                var filtro = request.ObterObjetoFiltro<FiltroRelatorioLeituraComunicadosDto>();
                var relatorioDto = new RelatorioLeituraComunicadosDto();

                filtro.Turma = filtro.Turma == "-99" ? "" : filtro.Turma;

                await ObterFiltroRelatorio(relatorioDto, filtro, request.UsuarioLogadoRF);
                await ObterDadosRelatorio(relatorioDto, filtro);

                await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioEscolaAquiLeituraComunicados", relatorioDto, request.CodigoCorrelacao));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private async Task ObterDadosRelatorio(RelatorioLeituraComunicadosDto relatorioDto, FiltroRelatorioLeituraComunicadosDto filtro)
        {
            relatorioDto.LeituraComunicadoDto = await mediator.Send(new ObterDadosLeituraComunicadosQuery(filtro));
        }

        private async Task ObterFiltroRelatorio(RelatorioLeituraComunicadosDto relatorioDto, FiltroRelatorioLeituraComunicadosDto filtro, string usuarioLogadoRF)
        {
            var filtroRelatorio = new FiltroLeituraComunicadosDto();


            filtroRelatorio.Dre = await ObterNomeDre(filtro.CodigoDre);
            filtroRelatorio.Ue = await ObterNomeUe(filtro.CodigoUe);
            filtroRelatorio.Usuario = filtro.NomeUsuario;
            filtroRelatorio.RF = usuarioLogadoRF;

            if (filtro.DataInicio == null)
                filtro.DataInicio = new DateTime(filtro.Ano, 1, 1);

            if (filtro.DataFim == null)
                filtro.DataFim = new DateTime(filtro.Ano, 12, 31);

            relatorioDto.Filtro = filtroRelatorio;
        }

        private async Task<string> ObterNomeDre(string dreCodigo)
        {
            var dre = dreCodigo.Equals("-99") ? null :
                await mediator.Send(new ObterDrePorCodigoQuery(dreCodigo));

            return dre != null ? dre.Abreviacao : "Enviado para todas";
        }

        private async Task<string> ObterNomeUe(string ueCodigo)
        {
            var ue = ueCodigo.Equals("-99") ? null :
                await mediator.Send(new ObterUePorCodigoQuery(ueCodigo));

            return ue != null ? ue.NomeRelatorio : "Enviado para todas";
        }
    }
}
