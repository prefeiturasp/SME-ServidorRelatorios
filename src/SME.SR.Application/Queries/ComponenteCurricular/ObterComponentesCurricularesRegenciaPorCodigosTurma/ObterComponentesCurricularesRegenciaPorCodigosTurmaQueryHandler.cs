using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterComponentesCurricularesRegenciaPorCodigosTurmaQueryHandler : IRequestHandler<ObterComponentesCurricularesRegenciaPorCodigosTurmaQuery, IEnumerable<IGrouping<string, ComponenteCurricularPorTurmaRegencia>>>
    {
        private static readonly long[] IDS_COMPONENTES_REGENCIA = { 2, 7, 8, 89, 138 };
        private IMediator _mediator;
        private IAtribuicaoCJRepository _atribuicaoCJRepository;

        public ObterComponentesCurricularesRegenciaPorCodigosTurmaQueryHandler(IMediator mediator, IAtribuicaoCJRepository atribuicaoCJRepository)
        {
            this._mediator = mediator;
            this._atribuicaoCJRepository = atribuicaoCJRepository;
        }

        public async Task<IEnumerable<IGrouping<string, ComponenteCurricularPorTurmaRegencia>>> Handle(ObterComponentesCurricularesRegenciaPorCodigosTurmaQuery request, CancellationToken cancellationToken)
        {
            if (request.Usuario.EhProfessorCj())
                return await ObterComponentesCJ(request.Modalidade, request.CodigosTurma, request.CodigoUe, request.CdComponentesCurriculares, request.Usuario.CodigoRf);
            else
            {
                var componentesCurriculares = await _mediator.Send(new
                    ObterComponentesCurricularesPorCodigosTurmaLoginEPerfilQuery()
                {
                    CodigosTurma = request.CodigosTurma,
                    ComponentesCurricularesApiEol = request.ComponentesCurricularesApiEol,
                    GruposMatriz = request.GruposMatriz,
                    Usuario = request.Usuario
                });

                return Enumerable.DefaultIfEmpty(componentesCurriculares.Where(c => c.Regencia).GroupBy(c => c.CodigoTurma));
            }
        }

        private async Task<IEnumerable<IGrouping<string, ComponenteCurricularPorTurmaRegencia>>> ObterComponentesCJ(Modalidade modalidade, string[] codigosTurma, string ueId, long[] codigosComponentesCurriculares, string rf, bool ignorarDeParaRegencia = false)
        {
            IEnumerable<ComponenteCurricularPorTurmaRegencia> componentes = null;
            var atribuicoes = await _atribuicaoCJRepository.ObterPorFiltros(modalidade,
                null,
                ueId,
                0,
                rf,
                string.Empty,
                true,
                turmaIds: codigosTurma,
                componentesCurricularresId: codigosComponentesCurriculares);

            if (atribuicoes == null || !atribuicoes.Any())
                return null;

            var disciplinasEol = await _mediator.Send(new ObterComponentesCurricularesPorIdsQuery() { ComponentesCurricularesIds = atribuicoes.Select(a => a.DisciplinaId).Distinct().ToArray() });

            var componenteRegencia = disciplinasEol?.FirstOrDefault(c => c.Regencia);
            if (componenteRegencia == null || ignorarDeParaRegencia)
                return MapearComponentesPorAtribuicaoCJ(atribuicoes, disciplinasEol);

            var componentesRegencia = await _mediator.Send(new ObterComponentesCurricularesPorIdsQuery() { ComponentesCurricularesIds = IDS_COMPONENTES_REGENCIA });
            if (componentesRegencia != null)
                return MapearComponentesPorAtribuicaoCJ(atribuicoes, componentesRegencia);

            return Enumerable.DefaultIfEmpty(componentes.GroupBy(c => c.CodigoTurma));
        }

        private IEnumerable<IGrouping<string, ComponenteCurricularPorTurmaRegencia>> MapearComponentesPorAtribuicaoCJ(IEnumerable<AtribuicaoCJ> atribuicoesCJ, IEnumerable<ComponenteCurricularPorTurma> componentesEol)
        {
            var componentesPorAtribuicaoCJ = Enumerable.Empty<ComponenteCurricularPorTurmaRegencia>();

            foreach (var atribuicao in atribuicoesCJ)
            {
                componentesPorAtribuicaoCJ =
                    componentesPorAtribuicaoCJ.Concat(componentesEol.Select(componente =>
                {
                    return new ComponenteCurricularPorTurmaRegencia()
                    {
                        CodigoTurma = atribuicao.Turma.Codigo,
                        CodDisciplina = componente.CodDisciplina,
                        CodDisciplinaPai = componente.CodDisciplinaPai,
                        Disciplina = componente.Disciplina,
                        Regencia = componente.Regencia,
                        Compartilhada = componente.Compartilhada,
                        Frequencia = componente.Frequencia,
                        LancaNota = componente.LancaNota,
                        TerritorioSaber = componente.TerritorioSaber,
                        BaseNacional = componente.BaseNacional,
                        GrupoMatriz = componente.GrupoMatriz
                    };
                }));
            }

            return Enumerable.DefaultIfEmpty(componentesPorAtribuicaoCJ.GroupBy(c => c.CodigoTurma));
        }
    }
}
