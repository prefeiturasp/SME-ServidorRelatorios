using MediatR;
using Nest;
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
    public class ObterComponentesCurricularesPorCodigosTurmaQueryHandler : IRequestHandler<ObterComponentesCurricularesPorCodigosTurmaQuery, IEnumerable<ComponenteCurricular>>
    {
        private readonly IComponenteCurricularRepository componenteCurricularRepository;
        private readonly IMediator mediator;
        private const long CODIGO_COMPONENTE_CURRICULAR_EDFISICA = 6;


        public ObterComponentesCurricularesPorCodigosTurmaQueryHandler(IComponenteCurricularRepository componenteCurricularRepository,
                                                                       IMediator mediator)
        {
            this.componenteCurricularRepository = componenteCurricularRepository ?? throw new ArgumentNullException(nameof(componenteCurricularRepository)); ;
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator)); ;
        }

        public async Task<IEnumerable<ComponenteCurricular>> Handle(ObterComponentesCurricularesPorCodigosTurmaQuery request, CancellationToken cancellationToken)
        {
            List<ComponenteCurricular> componentesCurriculares = await ObterComponentesCurriculares(request.CodigosTurma);
            PreencherComponenteCurricularEhTerritorio(componentesCurriculares, request.ComponentesCurriculares);

            await AdicionarComponentesTerritorio(request.CodigosTurma, componentesCurriculares);
            await AdicionarComponentesPlanejamento(componentesCurriculares, request.ComponentesCurriculares);

            if (request.EhEJA)
            {
                var componenteEdFisicaRegencia = componentesCurriculares.Find(w => w.Codigo == CODIGO_COMPONENTE_CURRICULAR_EDFISICA && w.ComponentePlanejamentoRegencia);

                if (componenteEdFisicaRegencia != null)
                    componentesCurriculares.Remove(componenteEdFisicaRegencia);
            }

            componentesCurriculares.ForEach(cc =>
            {
                var informacaoComponenteCurricular = request.ComponentesCurriculares.FirstOrDefault(x => x.Codigo == cc.Codigo || x.Codigo == cc.CodigoComponenteCurricularTerritorioSaber);
                cc.CodComponentePai = cc.CodigoComponentePai(request.ComponentesCurriculares);
                cc.Compartilhada = cc.EhCompartilhada(request.ComponentesCurriculares);
                cc.Descricao = cc.Descricao.Trim();
                cc.LancaNota = cc.PodeLancarNota(request.ComponentesCurriculares);
                cc.Frequencia = cc.ControlaFrequencia(request.ComponentesCurriculares);
                cc.ComponentePlanejamentoRegencia = cc.EhRegencia(request.ComponentesCurriculares) || cc.ComponentePlanejamentoRegencia;
                cc.TerritorioSaber = cc.TerritorioSaber;
                cc.BaseNacional = informacaoComponenteCurricular?.BaseNacional ?? false;
                cc.GrupoMatrizId = informacaoComponenteCurricular?.GrupoMatrizId ?? 0;
            });

            return componentesCurriculares;
        }
        

        private async Task AdicionarComponentesPlanejamento(List<ComponenteCurricular> componentesCurriculares, IEnumerable<InformacaoPedagogicaComponenteCurricularSGPDTO> informacoesComponentesCurriculares)
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

            var componentes = await componenteCurricularRepository.ListarInformacoesPedagogicasComponentesCurriculares();
            return componentes.ToComponentesCurriculares().Where(c => ids.Contains(c.Codigo));
        }

        private void PreencherComponenteCurricularEhTerritorio(List<ComponenteCurricular> componentesCurriculares, IEnumerable<InformacaoPedagogicaComponenteCurricularSGPDTO> informacoesComponentesCurriculares)
        {
            componentesCurriculares.ForEach(c =>
            {
                var informacaoComponenteCurricular = informacoesComponentesCurriculares.FirstOrDefault(cc => cc.Codigo == c.Codigo);
                c.TerritorioSaber = informacaoComponenteCurricular?.EhTerritorioSaber ?? false;
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
