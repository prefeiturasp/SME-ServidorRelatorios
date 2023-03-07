using MediatR;
using SME.SR.Application.Interfaces;
using SME.SR.Infra;
using SME.SR.Infra.Utilitarios;
using System;
using System.Collections.Generic;
using System.Linq;
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

                if (filtro.ComponentesCurriculares.Any(c => c == -99))
                    filtro.ComponentesCurriculares = Array.Empty<long>();

                var relatorioDto = new RelatorioAlteracaoNotasDto();

                await ObterFiltroRelatorio(relatorioDto, filtro, request.UsuarioLogadoRF);

                await ObterDadosRelatorio(relatorioDto, filtro);

                await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioHistoricoAlteracoesNotas", relatorioDto, request.CodigoCorrelacao));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private async Task ObterDadosRelatorio(RelatorioAlteracaoNotasDto relatorioDto, FiltroRelatorioAlteracaoNotasDto filtro)
        {
            relatorioDto.Turmas = await mediator.Send(new ObterDadosRelatorioAlteracaoNotasCommand(filtro));
        }

        private async Task<FiltroAlteracaoNotasDto> ObterFiltroRelatorio(RelatorioAlteracaoNotasDto relatorioDto, FiltroRelatorioAlteracaoNotasDto filtro, string usuarioLogadoRF)
        {
            return new FiltroAlteracaoNotasDto
            {
                Dre = await ObterNomeDre(filtro.CodigoDre),
                Ue = await ObterNomeUe(filtro.CodigoUe),
                Usuario = filtro.NomeUsuario,
                RF = usuarioLogadoRF,
                Bimestre = ObterNomeBimestre(filtro.Bimestres),
                ComponenteCurricular = await ObterComponenteCurricular(filtro.ComponentesCurriculares),
                Turma = await ObterNomeTurma(filtro.Turma)
            };
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

        private async Task<string> ObterComponenteCurricular(IEnumerable<long> componenteCurricularIds)
        {
            return !componenteCurricularIds.Any() ? "Todos" : componenteCurricularIds.Count() > 1
                ? string.Empty : await mediator.Send(new ObterNomeComponenteCurricularPorIdQuery(componenteCurricularIds.FirstOrDefault()));
        }

        private string ObterNomeBimestre(IEnumerable<int> bimestres)
        {
            var bimestre = string.Empty;
            var selecionouTodos = bimestres.Any(c => c == -99);
            var selecionouBimestreFinal = bimestres.Any(c => c == 0);

            bimestre = selecionouTodos ?
                "Todos"
                :
                bimestres.Count() > 1 ?
                string.Empty
                :
                selecionouBimestreFinal ?
                "Final"
                :
                bimestres.FirstOrDefault().ToString();

            return bimestre;
        }
        private async Task<string> ObterNomeTurma(IEnumerable<long> turmas)
        {
            var turmaNome = string.Empty;
            var turmaId = turmas.First();
            var selecionouTodos = turmas.Any(c => c == -99);

            turmaNome = selecionouTodos ?
                "Todas"
                :
                turmas.Count() > 1 ?
                string.Empty
                :
                await OberterNomeTurmaFormatado(turmaId);

            return turmaNome;
        }

        private async Task<string> OberterNomeTurmaFormatado(long turmaId)
        {
            var turma = await mediator.Send(new ObterTurmaResumoComDreUePorIdQuery(turmaId));

            return $"{turma.Modalidade.ShortName()} - {turma.Nome}";
        }
    }
}