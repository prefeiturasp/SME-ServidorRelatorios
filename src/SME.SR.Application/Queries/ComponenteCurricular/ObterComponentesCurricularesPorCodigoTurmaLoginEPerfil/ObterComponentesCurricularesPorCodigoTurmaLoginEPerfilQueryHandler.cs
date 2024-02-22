using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using SME.SR.Infra.Utilitarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterComponentesCurricularesPorCodigoTurmaLoginEPerfilQueryHandler : IRequestHandler<ObterComponentesCurricularesPorCodigoTurmaLoginEPerfilQuery, IEnumerable<ComponenteCurricularPorTurma>>
    {
        private readonly IComponenteCurricularRepository componenteCurricularRepository;
        private readonly IPermissaoRepository permissaoRepository;
        private readonly IMediator mediator;

        public ObterComponentesCurricularesPorCodigoTurmaLoginEPerfilQueryHandler(IComponenteCurricularRepository componenteCurricularRepository, IPermissaoRepository permissaoRepository, IMediator mediator)
        {
            this.componenteCurricularRepository = componenteCurricularRepository ?? throw new ArgumentNullException(nameof(componenteCurricularRepository));
            this.permissaoRepository = permissaoRepository ?? throw new ArgumentNullException(nameof(permissaoRepository));
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

        }

        public async Task<IEnumerable<ComponenteCurricularPorTurma>> Handle(ObterComponentesCurricularesPorCodigoTurmaLoginEPerfilQuery request, CancellationToken cancellationToken)
        {
            List<ComponenteCurricular> componentesCurriculares = await ObterComponentesCurriculares(request.Usuario.Login, request.Usuario.PerfilAtual, request.CodigoTurma);

            var informacoesComponentesCurriculares = await componenteCurricularRepository
                .ListarInformacoesPedagogicasComponentesCurriculares();

            var gruposMatriz = await componenteCurricularRepository
                .ListarGruposMatriz();

            PreencherComponenteCurricularEhTerritorio(componentesCurriculares, informacoesComponentesCurriculares);

            await AdicionarComponentesTerritorio(request.CodigoTurma, componentesCurriculares);
            await AdicionarComponentesPlanejamento(componentesCurriculares, informacoesComponentesCurriculares);

            return MapearParaDto(componentesCurriculares.DistinctBy(c => c.Codigo), informacoesComponentesCurriculares, gruposMatriz);
        }
        private IEnumerable<ComponenteCurricularPorTurma> MapearParaDto(IEnumerable<ComponenteCurricular> componentesCurriculares, IEnumerable<InformacaoPedagogicaComponenteCurricularSGPDTO> informacoesComponentesCurriculares, IEnumerable<Data.ComponenteCurricularGrupoMatriz> grupoMatrizes)
        {
            return componentesCurriculares?
                .Select(c => MapearParaDto(c, informacoesComponentesCurriculares, grupoMatrizes));
        }

        private ComponenteCurricularPorTurma MapearParaDto(ComponenteCurricular componenteCurricular, IEnumerable<InformacaoPedagogicaComponenteCurricularSGPDTO> informacoesComponentesCurriculares, IEnumerable<ComponenteCurricularGrupoMatriz> grupoMatrizes)
        {
            var informacaoComponenteCurricular = informacoesComponentesCurriculares
                .FirstOrDefault(x => x.Codigo == componenteCurricular.Codigo);

            return new ComponenteCurricularPorTurma
            {
                CodDisciplina = componenteCurricular.Codigo,
                CodDisciplinaPai = componenteCurricular.CodigoComponentePai(informacoesComponentesCurriculares) ?? componenteCurricular.CodComponentePai,
                Compartilhada = componenteCurricular.EhCompartilhada(informacoesComponentesCurriculares),
                Disciplina = componenteCurricular.Descricao.Trim(),
                LancaNota = componenteCurricular.PodeLancarNota(informacoesComponentesCurriculares),
                Regencia = componenteCurricular.EhRegencia(informacoesComponentesCurriculares) || componenteCurricular.ComponentePlanejamentoRegencia,
                TerritorioSaber = componenteCurricular.TerritorioSaber,
                BaseNacional = informacaoComponenteCurricular?.BaseNacional ?? false,
                GrupoMatriz = grupoMatrizes.FirstOrDefault(x => x.Id == informacaoComponenteCurricular?.GrupoMatrizId)
            };
        }

        private void PreencherComponenteCurricularEhTerritorio(List<ComponenteCurricular> componentesCurriculares, IEnumerable<InformacaoPedagogicaComponenteCurricularSGPDTO> informacoesComponentesCurriculares)
        {
            componentesCurriculares.ForEach(c =>
            {
                var informacaoComponenteCurricular = informacoesComponentesCurriculares.FirstOrDefault(cc => cc.Codigo == c.Codigo);
                c.TerritorioSaber = informacaoComponenteCurricular?.EhTerritorioSaber ?? false;
            });
        }

        private async Task AdicionarComponentesPlanejamento(List<ComponenteCurricular> componentesCurriculares, IEnumerable<InformacaoPedagogicaComponenteCurricularSGPDTO> informacoesComponentesCurriculares)
        {
            var componentesRegencia = componentesCurriculares.Where(c => c.EhRegencia(informacoesComponentesCurriculares));
            if (componentesRegencia != null && componentesRegencia.Any())
            {
                var idsComponentesPlanejamento = new List<(long codigo, long codigoComponentePai)>();
                var componentesRegenciaApiEol = await componenteCurricularRepository.ListarRegencia();

                foreach (var componenteRegencia in componentesRegencia)
                {
                    var componentesPlanejamento = componentesRegenciaApiEol.Where(r => r.Ano.HasValue &&
                                                                                       r.Ano.Value.ToString() == componenteRegencia.AnoTurma &&
                                                                                       r.Turno == componenteRegencia.TurnoTurma);

                    if (componentesPlanejamento == null || !componentesPlanejamento.Any())
                        componentesPlanejamento = componentesRegenciaApiEol.Where(r => !r.Ano.HasValue && !r.Turno.HasValue);

                    idsComponentesPlanejamento.AddRange(componentesPlanejamento.Select(c => (c.IdComponenteCurricular, componenteRegencia.Codigo)));
                }

                if (idsComponentesPlanejamento.Any())
                {
                    componentesCurriculares.RemoveAll(c => c.EhRegencia(informacoesComponentesCurriculares));
                    var componentes = (await ObterPorId(idsComponentesPlanejamento?.ToArray())).ToList();

                    if (componentes != null && componentes.Any())
                    {
                        for (int i = 0; i < componentes.Count; i++)
                            componentes[i].ComponentePlanejamentoRegencia = true;
                    }
                        
                    componentesCurriculares.AddRange(componentes);
                }
            }
        }

        private async Task<IEnumerable<ComponenteCurricular>> ObterPorId((long codigo, long codigoComponentePai)[] ids)
        {
            if (ids == null || !ids.Any())
                return Enumerable.Empty<ComponenteCurricular>();

            var componentes = await componenteCurricularRepository
                .ListarInformacoesPedagogicasComponentesCurriculares();

            return componentes
                .ToComponentesCurriculares(ids)
                .Where(c => ids.Select(i => i.codigo)
                .Contains(c.Codigo));
        }

        private async Task AdicionarComponentesTerritorio(string codigoTurma, List<ComponenteCurricular> componentesCurriculares)
        {
            _ = await mediator.Send(new AdicionarComponentesCurricularesTerritorioSaberTurmaQuery(new string[] { codigoTurma }, componentesCurriculares));
        }

        private async Task<List<ComponenteCurricular>> ObterComponentesCurriculares(string login, Guid idPerfil, string codigoTurma)
        {
            var componentesCurriculares = new List<ComponenteCurricular>();

            var gruposAbrangenciaApiEol = await permissaoRepository.ObterTodosGrupos();

            var grupoAbrangencia = gruposAbrangenciaApiEol.FirstOrDefault(c => c.GrupoID == idPerfil);
            if (grupoAbrangencia != null)
            {
                if (grupoAbrangencia.Abrangencia == TipoAbrangencia.Professor)
                    componentesCurriculares.AddRange(await componenteCurricularRepository.ObterComponentesPorTurmaEProfessor(login, codigoTurma));
                else
                {
                    var componentesDaTurma = await componenteCurricularRepository.ObterComponentesPorTurma(codigoTurma);
                    componentesCurriculares.AddRange(componentesDaTurma);
                }
                AdicionarComponentesProfessorEmebs(componentesCurriculares);
            }
            return componentesCurriculares;
        }

        private void AdicionarComponentesProfessorEmebs(List<ComponenteCurricular> componentesCurriculares)
        {
            bool profLibras = componentesCurriculares.Any(d => d.Codigo == 218 && d.TipoEscola == "4") && !componentesCurriculares.Any(d => d.Codigo == 138 && d.TipoEscola == "4");
            bool profPortugues = componentesCurriculares.Any(d => d.Codigo == 138 && d.TipoEscola == "4") && !componentesCurriculares.Any(d => d.Codigo == 218 && d.TipoEscola == "4");

            if (profLibras)
            {
                componentesCurriculares.Add(new ComponenteCurricular()
                {
                    Codigo = 138,
                    Descricao = "LINGUA PORTUGUESA",
                    TipoEscola = "4"
                });
            }
            else if (profPortugues)
            {
                componentesCurriculares.Add(new ComponenteCurricular()
                {
                    Codigo = 218,
                    Descricao = "LIBRAS",
                    TipoEscola = "4"
                });
            }
        }
    }
}
