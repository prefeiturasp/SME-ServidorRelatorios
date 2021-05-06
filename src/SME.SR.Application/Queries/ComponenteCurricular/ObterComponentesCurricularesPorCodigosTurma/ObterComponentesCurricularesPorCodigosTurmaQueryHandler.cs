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

        public ObterComponentesCurricularesPorCodigosTurmaQueryHandler(IComponenteCurricularRepository componenteCurricularRepository, IPermissaoRepository permissaoRepository)
        {
            this.componenteCurricularRepository = componenteCurricularRepository ?? throw new ArgumentNullException(nameof(componenteCurricularRepository)); ;
        }

        public async Task<IEnumerable<ComponenteCurricularPorTurmaRegencia>> Handle(ObterComponentesCurricularesPorCodigosTurmaQuery request, CancellationToken cancellationToken)
        {
            List<ComponenteCurricular> componentesCurriculares = await ObterComponentesCurriculares(request.CodigosTurma);

            await AdicionarComponentesTerritorio(request.CodigosTurma, componentesCurriculares, request.ComponentesCurriculares);

            await AdicionarComponentesPlanejamento(componentesCurriculares, request.ComponentesCurriculares);

            return MapearParaDto(componentesCurriculares, request.ComponentesCurriculares, request.GruposMatriz);
        }
        private IEnumerable<ComponenteCurricularPorTurmaRegencia> MapearParaDto(IEnumerable<Data.ComponenteCurricular> componentesCurriculares, IEnumerable<ComponenteCurricular> componentesApiEol, IEnumerable<Data.ComponenteCurricularGrupoMatriz> grupoMatrizes)
        {
            return componentesCurriculares?.Select(c => MapearParaDto(c, componentesApiEol, grupoMatrizes));
        }

        private ComponenteCurricularPorTurmaRegencia MapearParaDto(ComponenteCurricular componenteCurricular, IEnumerable<ComponenteCurricular> componentesApiEol, IEnumerable<ComponenteCurricularGrupoMatriz> grupoMatrizes)
        {
            var componenteCurricularEol = componentesApiEol.FirstOrDefault(x => x.Codigo == componenteCurricular.Codigo);

            return new ComponenteCurricularPorTurmaRegencia
            {
                CodigoTurma = componenteCurricular.CodigoTurma,
                CodDisciplina = componenteCurricular.Codigo,
                CodDisciplinaPai = componenteCurricular.CodigoComponentePai(componentesApiEol),
                Compartilhada = componenteCurricular.EhCompartilhada(componentesApiEol),
                Disciplina = componenteCurricular.Descricao.Trim(),
                LancaNota = componenteCurricular.PodeLancarNota(componentesApiEol),
                Frequencia = componenteCurricular.ControlaFrequencia(componentesApiEol),
                Regencia = componenteCurricular.EhRegencia(componentesApiEol) || componenteCurricular.ComponentePlanejamentoRegencia,
                TerritorioSaber = componenteCurricular.TerritorioSaber,
                BaseNacional = componenteCurricularEol?.BaseNacional ?? false,
                GrupoMatriz = grupoMatrizes.FirstOrDefault(x => x.Id == componenteCurricularEol?.GrupoMatrizId)
            };
        }

        private async Task AdicionarComponentesPlanejamento(List<ComponenteCurricular> componentesCurriculares, IEnumerable<ComponenteCurricular> componentesApiEol)
        {
            var componentesRegencia = componentesCurriculares.Where(c => c.EhRegencia(componentesApiEol));
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
                    componentesCurriculares.RemoveAll(c => c.EhRegencia(componentesApiEol));

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

        private async Task AdicionarComponentesTerritorio(string[] codigosTurma, List<ComponenteCurricular> componentesCurriculares, IEnumerable<ComponenteCurricular> componentesApiEol)
        {
            var componentesTerritorioApiEol = componentesApiEol?.Where(x => x.TerritorioSaber)?.ToList();
            if (componentesTerritorioApiEol != null && componentesTerritorioApiEol.Any())
            {
                var codigoDisciplinasTerritorio = componentesCurriculares.Where(x => componentesTerritorioApiEol.Any(z => x.Codigo == z.Codigo))?.Select(t => t.Codigo)?.Distinct();

                if (codigoDisciplinasTerritorio != null && codigoDisciplinasTerritorio.Any())
                {
                    var territoriosBanco = await componenteCurricularRepository.ObterComponentesTerritorioDosSaberes(codigosTurma, codigoDisciplinasTerritorio);
                    if (territoriosBanco != null && territoriosBanco.Any())
                    {
                        var tipoEscola = componentesCurriculares.FirstOrDefault().TipoEscola;

                        foreach (var territorio in territoriosBanco.GroupBy(t => t.CodigoTurma))
                        {
                            componentesCurriculares.RemoveAll(c => territoriosBanco.Any(x => x.CodigoComponenteCurricular == c.Codigo && c.CodigoTurma == territorio.Key));

                            var territorios = territorio.GroupBy(c => new { c.CodigoTerritorioSaber, c.CodigoExperienciaPedagogica, c.DataInicio });

                            foreach (var componenteTerritorio in territorios)
                            {
                                componentesCurriculares.Add(new Data.ComponenteCurricular()
                                {
                                    CodigoTurma = territorio.Key,
                                    Codigo = componenteTerritorio.FirstOrDefault().ObterCodigoComponenteCurricular(territorio.Key),
                                    Descricao = componenteTerritorio.FirstOrDefault().ObterDescricaoComponenteCurricular(),
                                    TipoEscola = tipoEscola,
                                    TerritorioSaber = true
                                });
                            }
                        }
                    }
                }
            }
        }

        private async Task<List<ComponenteCurricular>> ObterComponentesCurriculares(string[] codigosTurma)
        {
            var componentesCurriculares = new List<ComponenteCurricular>();

            var componentesDaTurma = await componenteCurricularRepository.ObterComponentesPorTurmas(codigosTurma);
            componentesCurriculares.AddRange(componentesDaTurma);

            return componentesCurriculares;
        }
    }
}
