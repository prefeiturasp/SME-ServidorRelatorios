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
    public class ObterComponentesCurricularesPorCodigosTurmaLoginEPerfilQueryHandler : IRequestHandler<ObterComponentesCurricularesPorCodigosTurmaLoginEPerfilQuery, IEnumerable<ComponenteCurricularPorTurmaRegencia>>
    {
        private readonly IComponenteCurricularRepository componenteCurricularRepository;
        private readonly IPermissaoRepository permissaoRepository;

        public ObterComponentesCurricularesPorCodigosTurmaLoginEPerfilQueryHandler(IComponenteCurricularRepository componenteCurricularRepository, IPermissaoRepository permissaoRepository)
        {
            this.componenteCurricularRepository = componenteCurricularRepository ?? throw new ArgumentNullException(nameof(componenteCurricularRepository)); ;
            this.permissaoRepository = permissaoRepository ?? throw new ArgumentNullException(nameof(permissaoRepository)); ;
        }

        public async Task<IEnumerable<ComponenteCurricularPorTurmaRegencia>> Handle(ObterComponentesCurricularesPorCodigosTurmaLoginEPerfilQuery request, CancellationToken cancellationToken)
        {
            List<ComponenteCurricular> componentesCurriculares = await ObterComponentesCurriculares(request.Usuario.Login, request.Usuario.PerfilAtual, request.CodigosTurma, request.ValidarAbrangenciaProfessor);

            await AdicionarComponentesTerritorio(request.CodigosTurma, componentesCurriculares, request.ComponentesCurriculares);

            await AdicionarComponentesPlanejamento(componentesCurriculares, request.ComponentesCurriculares);

            if (request.EhEJA)
                componentesCurriculares = componentesCurriculares.Where(w => w.Codigo != 6).ToList();

            return MapearParaDto(componentesCurriculares, request.ComponentesCurriculares, request.GruposMatriz);
        }
        private IEnumerable<ComponenteCurricularPorTurmaRegencia> MapearParaDto(IEnumerable<Data.ComponenteCurricular> componentesCurriculares, IEnumerable<ComponenteCurricular> componentesApiEol, IEnumerable<Data.ComponenteCurricularGrupoMatriz> grupoMatrizes)
        {
            return componentesCurriculares?.Select(c => MapearParaDto(c, componentesApiEol, grupoMatrizes));
        }

        private ComponenteCurricularPorTurmaRegencia MapearParaDto(ComponenteCurricular componenteCurricular, IEnumerable<ComponenteCurricular> componentesApiEol, IEnumerable<ComponenteCurricularGrupoMatriz> grupoMatrizes)
        {
            var componenteCurricularEol = componentesApiEol.FirstOrDefault(x => x.Codigo == componenteCurricular.Codigo || x.Codigo == componenteCurricular.CodigoTerritorioSaber);

            return new ComponenteCurricularPorTurmaRegencia
            {
                CodigoTurma = componenteCurricular.CodigoTurma,
                CodDisciplina = componenteCurricular.Codigo,
                CodigoTerritorioSaber = componenteCurricular.CodigoTerritorioSaber,
                CodDisciplinaPai = componenteCurricular.CodigoComponentePai(componentesApiEol),
                Compartilhada = componenteCurricular.EhCompartilhada(componentesApiEol),
                Disciplina = componenteCurricular.Descricao.Trim(),
                LancaNota = componenteCurricular.PodeLancarNota(componentesApiEol),
                Frequencia = componenteCurricular.ControlaFrequencia(componentesApiEol),
                Regencia = componenteCurricular.EhRegencia(componentesApiEol) || componenteCurricular.ComponentePlanejamentoRegencia,
                TerritorioSaber = componenteCurricular.TerritorioSaber,
                BaseNacional = componenteCurricularEol?.BaseNacional ?? false,
                GrupoMatriz = grupoMatrizes.FirstOrDefault(x => x.Id == componenteCurricularEol?.GrupoMatrizId),
                OrdemComponenteTerritorioSaber = componenteCurricular.OrdemTerritorioSaber,
                Professor = componenteCurricular.Professor
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

                        var componentesTerritorio = new List<(long codigo, string login, long grupoMatrizId)>();
                        componentesTerritorio
                            .AddRange(componentesCurriculares
                                .Where(x => componentesTerritorioApiEol.Any(z => x.Codigo == z.Codigo))
                                    .Select(t => (t.Codigo, t.Professor, componentesTerritorioApiEol.FirstOrDefault(z => t.Codigo == z.Codigo)?.GrupoMatrizId ?? 0)).ToList());

                        territoriosBanco = territoriosBanco.OrderBy(o => o.CodigoTerritorioSaber).ThenBy(t => t.CodigoExperienciaPedagogica);

                        foreach (var territorio in territoriosBanco.GroupBy(t => t.CodigoTurma))
                        {
                            componentesCurriculares.RemoveAll(c => territoriosBanco.Any(x => x.CodigoComponenteCurricular == c.Codigo && c.CodigoTurma == territorio.Key));

                            var territoriosComProfessores = DefinirProfessoresTerritorio(territoriosBanco, componentesTerritorio);
                            var territorios = territoriosComProfessores.GroupBy(c => new { c.CodigoTerritorioSaber, c.CodigoExperienciaPedagogica, c.DataInicio, c.Professor });

                            var ordemComponentesTerritorioSaber = 0;

                            foreach (var componenteTerritorio in territorios)
                            {
                                ordemComponentesTerritorioSaber++;                                
                                componentesCurriculares.Add(new Data.ComponenteCurricular()
                                {
                                    CodigoTurma = territorio.Key,
                                    Codigo = componenteTerritorio.FirstOrDefault()?.ObterCodigoComponenteCurricular(componenteTerritorio.First().CodigoTurma) ?? 0,
                                    CodigoTerritorioSaber = componenteTerritorio.FirstOrDefault()?.CodigoComponenteCurricular ?? 0,
                                    Descricao = componenteTerritorio.FirstOrDefault()?.ObterDescricaoComponenteCurricular(),
                                    TipoEscola = tipoEscola,
                                    TerritorioSaber = true,
                                    OrdemTerritorioSaber = ordemComponentesTerritorioSaber,
                                    GrupoMatrizId = componenteTerritorio.FirstOrDefault()?.GrupoMatrizId ?? 0,
                                    Professor = componenteTerritorio.FirstOrDefault()?.Professor
                                });
                            }
                        }
                    }
                }
            }
        }

        private List<ComponenteCurricularTerritorioSaber> DefinirProfessoresTerritorio(IEnumerable<ComponenteCurricularTerritorioSaber> territoriosBanco, IEnumerable<(long Codigo, string Login, long grupoMatrizId)> componentesTerritorio)
        {
            var territoriosComProfessores = new List<ComponenteCurricularTerritorioSaber>();
            territoriosBanco.ToList().ForEach(ts =>
            {
                var componenteEquivalente = componentesTerritorio?.FirstOrDefault(ct => ct.Codigo.Equals(ts.CodigoComponenteCurricular));
                territoriosComProfessores.Add(new ComponenteCurricularTerritorioSaber()
                {
                    CodigoTurma = ts.CodigoTurma,
                    CodigoExperienciaPedagogica = ts.CodigoExperienciaPedagogica,
                    CodigoTerritorioSaber = ts.CodigoTerritorioSaber,
                    DescricaoTerritorioSaber = ts.DescricaoTerritorioSaber,
                    DescricaoExperienciaPedagogica = ts.DescricaoExperienciaPedagogica,
                    DataInicio = ts.DataInicio = ts.DataInicio,
                    CodigoComponenteCurricular = ts.CodigoComponenteCurricular,     
                    GrupoMatrizId = componenteEquivalente?.grupoMatrizId ?? 0,
                    Professor = componenteEquivalente?.Login
                });
            });
            return territoriosComProfessores;
        }

        private async Task<List<ComponenteCurricular>> ObterComponentesCurriculares(string login, Guid idPerfil, string[] codigosTurma, bool validarAbrangenciaProfessor = true)
        {
            var componentesCurriculares = new List<ComponenteCurricular>();

            var gruposAbrangenciaApiEol = await permissaoRepository.ObterTodosGrupos();

            var grupoAbrangencia = gruposAbrangenciaApiEol.FirstOrDefault(c => c.GrupoID == idPerfil);
            if (grupoAbrangencia != null)
            {
                if (grupoAbrangencia.Abrangencia == TipoAbrangencia.Professor && validarAbrangenciaProfessor)
                {
                    componentesCurriculares.AddRange(await componenteCurricularRepository.ObterComponentesPorTurmasEProfessor(login, codigosTurma));
                }
                else
                {
                    var componentesDaTurma = await componenteCurricularRepository.ObterComponentesPorTurmasEProfessor(null, codigosTurma);
                    componentesCurriculares.AddRange(componentesDaTurma);
                }
                AdicionarComponentesProfessorEmebs(componentesCurriculares);
            }
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
