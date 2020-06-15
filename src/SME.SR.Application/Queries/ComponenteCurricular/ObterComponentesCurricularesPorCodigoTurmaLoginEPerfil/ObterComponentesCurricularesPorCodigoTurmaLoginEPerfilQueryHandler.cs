using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using SME.SR.Infra.Utilitarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application.Queries.ComponenteCurricular.ObterComponentesCurricularesPorCodigoTurmaLoginEPerfil
{
    public class ObterComponentesCurricularesPorCodigoTurmaLoginEPerfilQueryHandler : IRequestHandler<ObterComponentesCurricularesPorCodigoTurmaLoginEPerfilQuery, IEnumerable<ComponenteCurricularPorTurma>>
    {
        private IComponenteCurricularRepository _componenteCurricularRepository;
        private IPermissaoRepository _permissaoRepository;

        public ObterComponentesCurricularesPorCodigoTurmaLoginEPerfilQueryHandler(IComponenteCurricularRepository componenteCurricularRepository, IPermissaoRepository permissaoRepository)
        {
            this._componenteCurricularRepository = componenteCurricularRepository;
            this._permissaoRepository = permissaoRepository;
        }

        public async Task<IEnumerable<ComponenteCurricularPorTurma>> Handle(ObterComponentesCurricularesPorCodigoTurmaLoginEPerfilQuery request, CancellationToken cancellationToken)
        {
            List<Data.ComponenteCurricular> componentesCurriculares = await ObterComponentesCurriculares(request.Login, request.IdPerfil, request.CodigoTurma);

            var componentesApiEol = await _componenteCurricularRepository.Listar();
            var gruposMatriz = await _componenteCurricularRepository.ListarGruposMatriz();

            await AdicionarComponentesTerritorio(request.CodigoTurma, componentesCurriculares, componentesApiEol);

            return MapearParaDto(componentesCurriculares.DistinctBy(c => c.Codigo), componentesApiEol, gruposMatriz);
        }
        private IEnumerable<ComponenteCurricularPorTurma> MapearParaDto(IEnumerable<Data.ComponenteCurricular> componentesCurriculares, IEnumerable<ComponenteCurricularApiEol> componentesApiEol, IEnumerable<Data.ComponenteCurricularGrupoMatriz> grupoMatrizes)
        {
            return componentesCurriculares?.Select(c => MapearParaDto(c, componentesApiEol, grupoMatrizes));
        }

        private ComponenteCurricularPorTurma MapearParaDto(Data.ComponenteCurricular componenteCurricular, IEnumerable<ComponenteCurricularApiEol> componentesApiEol, IEnumerable<ComponenteCurricularGrupoMatriz> grupoMatrizes)
        {
            var componenteCurricularEol = componentesApiEol.FirstOrDefault(x => x.IdComponenteCurricular == componenteCurricular.Codigo);

            return new ComponenteCurricularPorTurma
            {
                CodDisciplina = componenteCurricular.Codigo,
                CodDisciplinaPai = componenteCurricular.CodigoComponentePai(componentesApiEol),
                Compartilhada = componenteCurricular.EhCompartilhada(componentesApiEol),
                Disciplina = componenteCurricular.Descricao.Trim(),
                LancaNota = componenteCurricular.PodeLancarNota(componentesApiEol),
                Regencia = componenteCurricular.EhRegencia(componentesApiEol) || componenteCurricular.ComponentePlanejamentoRegencia,
                TerritorioSaber = componenteCurricular.TerritorioSaber,
                BaseNacional = componenteCurricularEol?.EhBaseNacional ?? false,
                GrupoMatriz = grupoMatrizes.FirstOrDefault(x => x.Id == componenteCurricularEol?.IdGrupoMatriz)
            };
        }

        private async Task AdicionarComponentesTerritorio(string codigoTurma, List<Data.ComponenteCurricular> componentesCurriculares, IEnumerable<ComponenteCurricularApiEol> componentesApiEol)
        {
            var componentesTerritorioApiEol = componentesApiEol?.Where(x => x.EhTerritorio)?.ToList();
            if (componentesTerritorioApiEol != null && componentesTerritorioApiEol.Any())
            {
                var codigoDisciplinasTerritorio = componentesCurriculares.Where(x => componentesTerritorioApiEol.Any(z => x.Codigo == z.IdComponenteCurricular)).Select(t => t.Codigo);

                if (codigoDisciplinasTerritorio != null && codigoDisciplinasTerritorio.Any())
                {
                    var territoriosBanco = await _componenteCurricularRepository.ObterComponentesTerritorioDosSaberes(codigoTurma, codigoDisciplinasTerritorio);
                    if (territoriosBanco != null && territoriosBanco.Any())
                    {
                        var tipoEscola = componentesCurriculares.FirstOrDefault().TipoEscola;

                        componentesCurriculares.RemoveAll(c => territoriosBanco.Any(x => x.CodigoComponenteCurricular == c.Codigo));

                        var territorios = territoriosBanco.GroupBy(c => new { c.CodigoTerritorioSaber, c.CodigoExperienciaPedagogica, c.DataInicio });

                        foreach (var componenteTerritorio in territorios)
                        {
                            componentesCurriculares.Add(new Data.ComponenteCurricular()
                            {
                                Codigo = componenteTerritorio.FirstOrDefault().ObterCodigoComponenteCurricular(codigoTurma),
                                Descricao = componenteTerritorio.FirstOrDefault().ObterDescricaoComponenteCurricular(),
                                TipoEscola = tipoEscola,
                                TerritorioSaber = true
                            });
                        }
                    }
                }
            }
        }

        private async Task<List<Data.ComponenteCurricular>> ObterComponentesCurriculares(string login, Guid idPerfil, string codigoTurma)
        {
            var componentesCurriculares = new List<Data.ComponenteCurricular>();

            var gruposAbrangenciaApiEol = await _permissaoRepository.ObterTodosGrupos();

            var grupoAbrangencia = gruposAbrangenciaApiEol.FirstOrDefault(c => c.GrupoID == idPerfil);
            if (grupoAbrangencia != null)
            {
                if (grupoAbrangencia.Abrangencia == TipoAbrangencia.Professor)
                {
                    componentesCurriculares.AddRange(await _componenteCurricularRepository.ObterComponentesPorTurmaEProfessor(login, codigoTurma));
                }
                else
                {
                    var componentesDaTurma = await _componenteCurricularRepository.ObterComponentesPorTurma(codigoTurma);
                    componentesCurriculares.AddRange(componentesDaTurma);
                }
                AdicionarComponentesProfessorEmebs(componentesCurriculares);
            }
            return componentesCurriculares;
        }

        private void AdicionarComponentesProfessorEmebs(List<Data.ComponenteCurricular> componentesCurriculares)
        {
            bool profLibras = componentesCurriculares.Any(d => d.Codigo == 218 && d.TipoEscola == "4") && !componentesCurriculares.Any(d => d.Codigo == 138 && d.TipoEscola == "4");
            bool profPortugues = componentesCurriculares.Any(d => d.Codigo == 138 && d.TipoEscola == "4") && !componentesCurriculares.Any(d => d.Codigo == 218 && d.TipoEscola == "4");

            if (profLibras)
            {
                componentesCurriculares.Add(new Data.ComponenteCurricular()
                {
                    Codigo = 138,
                    Descricao = "LINGUA PORTUGUESA",
                    TipoEscola = "4"
                });
            }
            else if (profPortugues)
            {
                componentesCurriculares.Add(new Data.ComponenteCurricular()
                {
                    Codigo = 218,
                    Descricao = "LIBRAS",
                    TipoEscola = "4"
                });
            }
        }
    }
}
