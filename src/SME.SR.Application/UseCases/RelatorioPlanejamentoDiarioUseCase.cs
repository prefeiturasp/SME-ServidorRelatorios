using MediatR;
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
            request.RotaErro = RotasRabbitSGP.RotaRelatoriosComErroControlePlanejamentoDiario;

            var parametros = request.ObterObjetoFiltro<FiltroRelatorioPlanejamentoDiarioDto>();

            var utilizarLayoutNovo = await UtilizarNovoLayout(parametros.AnoLetivo);

            var relatorioDto = new RelatorioControlePlanejamentoDiarioDto { Filtro = await ObterFiltroRelatorio(parametros, request.UsuarioLogadoRF, utilizarLayoutNovo) };
            
            if (utilizarLayoutNovo)
                await RelatorioComComponenteCurricular(parametros, request, relatorioDto);
            else
                await RelatorioSemComponenteCurricular(parametros, request, relatorioDto);

        }

        private async Task<bool> UtilizarNovoLayout(int anoLetivo)
        {
            return (await mediator.Send(new ObterParametroSistemaPorTipoAnoQuery(anoLetivo, TipoParametroSistema.ControlePlanejamentoDiarioInfantilComComponente))) != null;
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
                relatorioDto.TurmasInfantisComComponente = await mediator.Send(new ObterDadosPlanejamentoDiarioBordoComComponenteQuery(parametros));
                await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioControlePlanejamentoDiarioInfantilComComponente", relatorioDto, request.CodigoCorrelacao));
            }
            else
            {
                relatorioDto.Turmas = await mediator.Send(new ObterPlanejamentoDiarioPlanoAulaQuery(parametros));
                await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioControlePlanejamentoDiario", relatorioDto, request.CodigoCorrelacao));
            }
        }

        private async Task<FiltroControlePlanejamentoDiarioDto> ObterFiltroRelatorio(FiltroRelatorioPlanejamentoDiarioDto parametros, string usuarioLogadoRF, bool utilizarLayoutNovo)
        {
            return new FiltroControlePlanejamentoDiarioDto()
            {
                Dre = await ObterDre(parametros.CodigoDre),
                Ue = await ObterUe(parametros.CodigoUe),
                Bimestre = parametros.Bimestre == -99 ? "Todos" : parametros.Bimestre.ToString(),
                ComponenteCurricular = utilizarLayoutNovo 
                                       ? parametros.ComponentesCurricularesDisponiveis.Count() == 1 ? await ObterComponenteCurricular(parametros.ComponentesCurricularesDisponiveis.FirstOrDefault()) : "Todos" 
                                       : await ObterComponenteCurricular(parametros.ComponenteCurricular, false),
                Turma = await ObterTurma(parametros.CodigoTurma),
                RF = usuarioLogadoRF,
                Usuario = parametros.UsuarioNome,
                ComponentesCurricularesDisponiveis = parametros.ComponentesCurricularesDisponiveis,
            };
        }

        private async Task<string> ObterTurma(string codigoTurma)
        {
            if (codigoTurma == "-99")
                return "Todos";

            var turma = await mediator.Send(new ObterTurmaQuery(codigoTurma));
            return turma.NomeRelatorio;
        }

        private async Task<string> ObterComponenteCurricular(long componenteCurricular, bool obterDescricaoInfatil = true)
        {
            if (componenteCurricular == -99)
                return "Todos";

            var componente = await mediator.Send(new ObterComponentesCurricularesEolPorIdsQuery(new long[] { componenteCurricular }));

            return componente != null && componente.Any() ?
                obterDescricaoInfatil ? componente.First().DescricaoInfatil : componente.First().Disciplina : "";
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