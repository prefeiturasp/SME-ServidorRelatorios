using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterComponentesCurricularesPorCodigosTurmaQueryHandler : IRequestHandler<ObterComponentesCurricularesPorCodigosTurmaQuery, IEnumerable<ComponenteCurricularPorTurmaRegencia>>
    {
        private readonly IComponenteCurricularRepository componenteCurricularRepository;
        private readonly IMediator mediator;

        public ObterComponentesCurricularesPorCodigosTurmaQueryHandler(IComponenteCurricularRepository componenteCurricularRepository,
                                                                       IMediator mediator)
        {
            this.componenteCurricularRepository = componenteCurricularRepository ?? throw new ArgumentNullException(nameof(componenteCurricularRepository)); ;
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator)); ;
        }

        public async Task<IEnumerable<ComponenteCurricularPorTurmaRegencia>> Handle(ObterComponentesCurricularesPorCodigosTurmaQuery request, CancellationToken cancellationToken)
        {
            List<ComponenteCurricular> componentesCurriculares = await ObterComponentesCurriculares(request.CodigosTurma);
            PreencherComponenteCurricularEhTerritorio(componentesCurriculares, request.ComponentesCurriculares);

            await AdicionarComponentesTerritorio(request.CodigosTurma, componentesCurriculares);
            await AdicionarComponentesPlanejamento(componentesCurriculares, request.ComponentesCurriculares);

            if (request.EhEJA)
            {
                var componenteEdFisicaRegencia = componentesCurriculares.Find(w => w.Codigo == 6 && w.ComponentePlanejamentoRegencia);

                if (componenteEdFisicaRegencia != null)
                    componentesCurriculares.Remove(componenteEdFisicaRegencia);
            }

            return MapearParaDto(componentesCurriculares, request.ComponentesCurriculares, request.GruposMatriz);
        }
        private IEnumerable<ComponenteCurricularPorTurmaRegencia> MapearParaDto(IEnumerable<Data.ComponenteCurricular> componentesCurriculares, IEnumerable<ComponenteCurricular> informacoesComponentesCurriculares, IEnumerable<Data.ComponenteCurricularGrupoMatriz> grupoMatrizes)
        {
            return componentesCurriculares?.Select(c => MapearParaDto(c, informacoesComponentesCurriculares, grupoMatrizes));
        }

        private ComponenteCurricularPorTurmaRegencia MapearParaDto(ComponenteCurricular componenteCurricular, IEnumerable<ComponenteCurricular> informacoesComponentesCurriculares, IEnumerable<ComponenteCurricularGrupoMatriz> grupoMatrizes)
        {
            var componenteCurricularEol = informacoesComponentesCurriculares.FirstOrDefault(x => x.Codigo == componenteCurricular.Codigo || x.Codigo == componenteCurricular.CodigoComponenteCurricularTerritorioSaber);

            return new ComponenteCurricularPorTurmaRegencia
            {
                CodigoTurma = componenteCurricular.CodigoTurma,
                CodDisciplina = componenteCurricular.Codigo,
                CodigoComponenteCurricularTerritorioSaber = componenteCurricular.CodigoComponenteCurricularTerritorioSaber,
                CodDisciplinaPai = componenteCurricular.CodigoComponentePai(informacoesComponentesCurriculares),
                Compartilhada = componenteCurricular.EhCompartilhada(informacoesComponentesCurriculares),
                Disciplina = componenteCurricular.Descricao.Trim(),
                LancaNota = componenteCurricular.PodeLancarNota(informacoesComponentesCurriculares),
                Frequencia = componenteCurricular.ControlaFrequencia(informacoesComponentesCurriculares),
                Regencia = componenteCurricular.EhRegencia(informacoesComponentesCurriculares) || componenteCurricular.ComponentePlanejamentoRegencia,
                TerritorioSaber = componenteCurricular.TerritorioSaber,
                BaseNacional = componenteCurricularEol?.BaseNacional ?? false,
                GrupoMatriz = grupoMatrizes.FirstOrDefault(x => x.Id == componenteCurricularEol?.GrupoMatrizId),
                Professor = componenteCurricular.Professor
            };
        }

        private async Task AdicionarComponentesPlanejamento(List<ComponenteCurricular> componentesCurriculares, IEnumerable<ComponenteCurricular> informacoesComponentesCurriculares)
        {
            var componentesRegencia = componentesCurriculares.Where(c => c.EhRegencia(informacoesComponentesCurriculares));
            if (componentesRegencia != null && componentesRegencia.Any())
            {
                var idsComponentesPlanejamento = new Dictionary<string, IEnumerable<long>>();
                var componentesRegenciaApiEol = await componenteCurricularRepository.ListarRegencia();
                foreach (var componentesRegenciaPorTurma in componentesRegencia.GroupBy(cr => new { cr.CodigoTurma, cr.AnoTurma, cr.TurnoTurma }))
                {
                    var componentesPlanejamento = componentesRegenciaApiEol.Where(r => r.Ano.HasValue &&
                                                                                       r.Ano.Value.ToString() == componentesRegenciaPorTurma.Key.AnoTurma &&
                                                                                       r.Turno == componentesRegenciaPorTurma.Key.TurnoTurma);

                    if (componentesPlanejamento == null || !componentesPlanejamento.Any())
                    {
                        componentesPlanejamento = componentesRegenciaApiEol.Where(r => !r.Ano.HasValue && !r.Turno.HasValue);
                    }

                    idsComponentesPlanejamento.Add(componentesRegenciaPorTurma.Key.CodigoTurma, componentesPlanejamento.Select(c => c.IdComponenteCurricular));
                }
                if (idsComponentesPlanejamento.Any())
                {
                    componentesCurriculares.RemoveAll(c => c.EhRegencia(informacoesComponentesCurriculares));

                    var componentes = await ObterPorId(idsComponentesPlanejamento.SelectMany(x => x.Value).Distinct()?.ToArray());
                    if (componentes != null && componentes.Any())
                    {
                        foreach (KeyValuePair<string, IEnumerable<long>> componentesPorTurma in idsComponentesPlanejamento)
                        {
                            var componentesParaInserir = componentes.Where(c => componentesPorTurma.Value.Contains(c.Codigo))
                                                                    .Select(x =>
                                                                    {
                                                                        var retorno = (ComponenteCurricular)x.Clone();
                                                                        retorno.CodigoTurma = componentesPorTurma.Key;
                                                                        retorno.ComponentePlanejamentoRegencia = true;
                                                                        return retorno;
                                                                    });

                            componentesCurriculares.AddRange(componentesParaInserir);                           
                        }
                    }
                }
            }

        }

        private async Task<IEnumerable<Data.ComponenteCurricular>> ObterPorId(long[] ids)
        {
            if (ids == null || !ids.Any())
                return Enumerable.Empty<Data.ComponenteCurricular>();

            var componentes = await componenteCurricularRepository.ListarComponentes();
            return componentes.Where(c => ids.Contains(c.Codigo));
        }

        private void PreencherComponenteCurricularEhTerritorio(List<ComponenteCurricular> componentesCurriculares, IEnumerable<ComponenteCurricular> informacoesComponentesCurriculares)
        {
            componentesCurriculares.ForEach(c =>
            {
                var informacaoComponenteCurricular = informacoesComponentesCurriculares.FirstOrDefault(cc => cc.Codigo == c.Codigo);
                c.TerritorioSaber = informacaoComponenteCurricular?.TerritorioSaber ?? false;
            });
        }

        private async Task AdicionarComponentesTerritorio(string[] codigosTurma, List<ComponenteCurricular> componentesCurriculares)
        {
            componentesCurriculares = await mediator.Send(new AdicionarComponentesCurricularesTerritorioSaberTurmaQuery(codigosTurma, componentesCurriculares));
        }

        private async Task<List<ComponenteCurricular>> ObterComponentesCurriculares(string[] codigosTurma)
        {
            var componentesCurriculares = new List<ComponenteCurricular>();

            var componentesDaTurma = await componenteCurricularRepository.ObterComponentesPorTurmas(codigosTurma);
            componentesCurriculares.AddRange(componentesDaTurma);

            AdicionarComponentesProfessorEmebs(componentesCurriculares);

            return componentesCurriculares;
        }

        private void AdicionarComponentesProfessorEmebs(List<ComponenteCurricular> componentesCurriculares)
        {
            IList<ComponenteCurricular> componentesParaAdd = new List<ComponenteCurricular>();

            foreach (var ccPorTurma in componentesCurriculares.GroupBy(cc => cc.CodigoTurma))
            {
                bool profLibras = ccPorTurma.Any(d => d.Codigo == 218 && d.TipoEscola == "4") && !ccPorTurma.Any(d => d.Codigo == 138 && d.TipoEscola == "4");
                bool profPortugues = ccPorTurma.Any(d => d.Codigo == 138 && d.TipoEscola == "4") && !ccPorTurma.Any(d => d.Codigo == 218 && d.TipoEscola == "4");

                if (profLibras)
                {
                    componentesParaAdd.Add(new ComponenteCurricular()
                    {
                        CodigoTurma = ccPorTurma.Key,
                        Codigo = 138,
                        Descricao = "LINGUA PORTUGUESA",
                        TipoEscola = "4"
                    });
                }
                else if (profPortugues)
                {
                    componentesParaAdd.Add(new ComponenteCurricular()
                    {
                        CodigoTurma = ccPorTurma.Key,
                        Codigo = 218,
                        Descricao = "LIBRAS",
                        TipoEscola = "4"
                    });
                }
            }

            if (componentesParaAdd.Any())
                componentesCurriculares.AddRange(componentesParaAdd);
        }
    }
}
