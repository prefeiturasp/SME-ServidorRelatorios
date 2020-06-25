using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static SME.SR.Infra.Enumeradores;

namespace SME.SR.Application
{
    public class RelatorioConselhoClasseAtaFinalUseCase : IRelatorioConselhoClasseAtaFinalUseCase
    {
        private readonly IMediator mediator;

        public RelatorioConselhoClasseAtaFinalUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Executar(FiltroRelatorioDto request)
        {
            try
            {
                var parametros = request.ObterObjetoFiltro<FiltroConselhoClasseAtaFinalDto>();

                var cabecalho = await mediator.Send(new ObterAtaFinalCabecalhoQuery(parametros.TurmaCodigo));
                var alunos = await ObterAlunos(parametros.TurmaCodigo);
                var componentesCurriculares = await ObterComponentesCurriculares(parametros.TurmaCodigo);

                await mediator.Send(new GerarRelatorioAssincronoCommand("/sgp/RelatorioConselhoAta/ConselhoAta", null, FormatoEnum.Pdf, request.CodigoCorrelacao));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task<IEnumerable<AlunoSituacaoAtaFinalDto>> ObterAlunos(string turmaCodigo)
        {
            var alunos = await mediator.Send(new ObterAlunosSituacaoPorTurmaQuery(turmaCodigo)); 
            return alunos.Select(a => new AlunoSituacaoAtaFinalDto(a));
        }

        private async Task<IEnumerable<ConselhoClasseAtaFinalComponenteDto>> ObterComponentesCurriculares(string turmaCodigo)
        {
            var componentesCurriculares = await mediator.Send(new ObterComponentesCurricularesPorTurmaQuery(turmaCodigo));

            return MapearComponentes(componentesCurriculares);
        }

        private IEnumerable<ConselhoClasseAtaFinalComponenteDto> MapearComponentes(IEnumerable<ComponenteCurricularPorTurma> componentes)
        {
            foreach (var componente in componentes)
                yield return MapearComponente(componente);
        }

        private ConselhoClasseAtaFinalComponenteDto MapearComponente(ComponenteCurricularPorTurma componente)
            => componente == null ? null :
            new ConselhoClasseAtaFinalComponenteDto()
            {
                Codigo = componente.CodDisciplina,
                Nome = componente.Disciplina,
                GrupoMatriz = componente.GrupoMatriz?.Nome
            };
    }
}
