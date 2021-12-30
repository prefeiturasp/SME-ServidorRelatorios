using MediatR;
using Sentry;
using SME.SR.Application.Interfaces;
using SME.SR.Infra;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class RelatorioPlanejamentoDiarioUseCase : IRelatorioPlanejamentoDiarioUseCase
    {
        private readonly IMediator mediator;

        public RelatorioPlanejamentoDiarioUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Executar(FiltroRelatorioDto request)
        {
            long utilizarNovoLayoutDepoisDoAno = 2022;
            try
            {
                var parametros = request.ObterObjetoFiltro<FiltroRelatorioPlanejamentoDiarioDto>();
                var relatorioDto = new RelatorioControlePlanejamentoDiarioDto { Filtro = await ObterFiltroRelatorio(parametros, request.UsuarioLogadoRF) };
                relatorioDto.Turmas = await mediator.Send(new ObterDadosPlanejamentoDiarioBordoQuery(parametros));

                if (parametros.AnoLetivo < utilizarNovoLayoutDepoisDoAno)
                    await RelatorioSemComponenteCurricular(parametros, request, relatorioDto);
                else
                    await RelatorioComComponenteCurricular(parametros, request, relatorioDto);
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
            }

        }
        private async Task RelatorioSemComponenteCurricular(FiltroRelatorioPlanejamentoDiarioDto parametros, FiltroRelatorioDto request, RelatorioControlePlanejamentoDiarioDto relatorioDto)
        {
            if (parametros.ModalidadeTurma == Modalidade.Infantil)
            {
                relatorioDto.Turmas = await mediator.Send(new ObterDadosPlanejamentoDiarioBordoQuery(parametros));
                await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioControlePlanejamentoDiarioInfantil", relatorioDto, request.CodigoCorrelacao));
            }
            else
            {
                relatorioDto.Turmas = await mediator.Send(new ObterPlanejamentoDiarioPlanoAulaQuery(parametros));
                await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioControlePlanejamentoDiario", relatorioDto, request.CodigoCorrelacao));
            }
        }
        private async Task RelatorioComComponenteCurricular(FiltroRelatorioPlanejamentoDiarioDto parametros, FiltroRelatorioDto request, RelatorioControlePlanejamentoDiarioDto relatorioDto)
        {
            if (parametros.ModalidadeTurma == Modalidade.Infantil)
            {
                relatorioDto.Turmas = await mediator.Send(new ObterDadosPlanejamentoDiarioBordoQuery(parametros));
                
                var relatorioComponenteDto = new RelatorioControlePlanejamentoDiarioComponenteDto{Filtro = relatorioDto.Filtro};
                await MapearComponeteCurricular(relatorioComponenteDto, relatorioDto);

                await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("LAYOUT_NOVO", relatorioComponenteDto, request.CodigoCorrelacao));
            }
            else
            {
                relatorioDto.Turmas = await mediator.Send(new ObterPlanejamentoDiarioPlanoAulaQuery(parametros));
                await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioControlePlanejamentoDiario", relatorioDto, request.CodigoCorrelacao));
            }
        }

        private async Task MapearComponeteCurricular(RelatorioControlePlanejamentoDiarioComponenteDto relatorioComponenteDto, RelatorioControlePlanejamentoDiarioDto relatorioDto)
        {
            throw new NotImplementedException();
        }

        private async Task<FiltroControlePlanejamentoDiarioDto> ObterFiltroRelatorio(FiltroRelatorioPlanejamentoDiarioDto parametros, string usuarioLogadoRF)
        {
            return new FiltroControlePlanejamentoDiarioDto()
            {
                Dre = await ObterDre(parametros.CodigoDre),
                Ue = await ObterUe(parametros.CodigoUe),
                Bimestre = parametros.Bimestre == -99 ? "Todos" : parametros.Bimestre.ToString(),
                ComponenteCurricular = await ObterComponenteCurricular(parametros.ComponenteCurricular),
                Turma = await ObterTurma(parametros.CodigoTurma),
                RF = usuarioLogadoRF,
                Usuario = parametros.UsuarioNome
            };
        }

        private async Task<string> ObterTurma(string codigoTurma)
        {
            if (codigoTurma == "-99")
                return "Todos";

            var turma = await mediator.Send(new ObterTurmaQuery(codigoTurma));
            return turma.NomeRelatorio;
        }

        private async Task<string> ObterComponenteCurricular(long componenteCurricular)
        {
            if (componenteCurricular == -99)
                return "Todos";

            var componente = await mediator.Send(new ObterComponentesCurricularesEolPorIdsQuery(new long[] { componenteCurricular }));

            return componente != null && componente.Any() ?
                componente.First().Disciplina : "";
        }

        private async Task<string> ObterUe(string codigoUe)
        {
            if (codigoUe == "-99")
                return "Todos";

            var ue = await mediator.Send(new ObterUePorCodigoQuery(codigoUe));
            return $"{ue.Codigo} - {ue.NomeComTipoEscola}";
        }

        private async Task<string> ObterDre(string codigoDre)
        {
            if (codigoDre == "-99")
                return "Todos";

            var dre = await mediator.Send(new ObterDrePorCodigoQuery(codigoDre));
            return dre.Abreviacao;
        }
    }
}