using MediatR;
using SME.SR.Data;
using SME.SR.Data.Extensions;
using SME.SR.Data.Interfaces;
using SME.SR.Infra.Utilitarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterComponentesCurricularesEolPorIdsQueryHandler : IRequestHandler<ObterComponentesCurricularesEolPorIdsQuery, IEnumerable<ComponenteCurricularPorTurma>>
    {
        private readonly IComponenteCurricularRepository componenteCurricularRepository;
        private readonly IAreaDoConhecimentoRepository areaDoConhecimentoRepository;

        public ObterComponentesCurricularesEolPorIdsQueryHandler(IComponenteCurricularRepository componenteCurricularRepository, IAreaDoConhecimentoRepository areaDoConhecimentoRepository)
        {
            this.componenteCurricularRepository = componenteCurricularRepository ?? throw new ArgumentNullException(nameof(componenteCurricularRepository));
            this.areaDoConhecimentoRepository = areaDoConhecimentoRepository ?? throw new ArgumentNullException(nameof(areaDoConhecimentoRepository));
        }

        public async Task<IEnumerable<ComponenteCurricularPorTurma>> Handle(ObterComponentesCurricularesEolPorIdsQuery request, CancellationToken cancellationToken)
        {
            var idsComponentesAgrupamentoTS = request.ComponentesCurricularesIds.Where(cc => cc.EhIdComponenteCurricularTerritorioSaberAgrupado()).Select(cc => cc);
            var componentesCurriculares = await componenteCurricularRepository.ListarComponentes();
            var componentesCurricularesRetorno = componentesCurriculares
                   .Where(w => request.ComponentesCurricularesIds.Contains(w.Codigo))
                   .ToList();

            var componentesTS = componentesCurricularesRetorno.Where(c => c.TerritorioSaber)
                                .Select(c => c.Codigo);

            var componenteIds = request.ComponentesCurricularesIds.Select(cc => cc).ToList();

            if (componentesTS.Any() && request.TurmasId.Any())
            {
                List<ComponenteCurricularTerritorioSaber> componentesTerritorioSaber = (await componenteCurricularRepository.ObterComponentesTerritorioDosSaberes(request.TurmasId, componentesTS)).ToList();
                componentesCurricularesRetorno = ConcatenarComponenteTerritorio(componentesCurricularesRetorno, componentesTerritorioSaber, componentesCurriculares);
            }

            if (idsComponentesAgrupamentoTS.Any())
            {
                List<AgrupamentoAtribuicaoTerritorioSaber> componentesAgrupamentoTerritorioSaber = (await componenteCurricularRepository.ObterAgrupamentosTerritorioSaber(idsComponentesAgrupamentoTS.ToArray())).ToList();
                componentesCurricularesRetorno = ConcatenarComponenteAgrupamentoTerritorio(componentesCurricularesRetorno, componentesAgrupamentoTerritorioSaber, componentesCurriculares);
                componenteIds.AddRange(componentesAgrupamentoTerritorioSaber.SelectMany(cca => cca.ComponentesCurricularesAgrupados).Distinct());
            }

            if (componentesCurricularesRetorno != null && componentesCurricularesRetorno.Any())
            {
                var gruposMatriz = await componenteCurricularRepository.ListarGruposMatriz();
                var areasDoConhecimento = await areaDoConhecimentoRepository.ObterAreasDoConhecimentoPorComponentesCurriculares(componenteIds.ToArray());

                return componentesCurricularesRetorno.Select(x => new ComponenteCurricularPorTurma
                {
                    CodDisciplina = x.Codigo,
                    CodDisciplinaPai = x.CodComponentePai,
                    Disciplina = x.Descricao.Trim(),
                    Regencia = x.ComponentePlanejamentoRegencia,
                    Compartilhada = x.Compartilhada,
                    Frequencia = x.Frequencia,
                    LancaNota = x.LancaNota,
                    TerritorioSaber = x.TerritorioSaber,
                    BaseNacional = x.BaseNacional,
                    GrupoMatriz = x.ObterGrupoMatriz(gruposMatriz),
                    AreaDoConhecimento = x.ObterAreaDoConhecimento(areasDoConhecimento),
                    DescricaoInfatil = x.DescricaoInfantil
                });
            }

            return Enumerable.Empty<ComponenteCurricularPorTurma>();
        }

        public List<ComponenteCurricular> ConcatenarComponenteTerritorio(IEnumerable<ComponenteCurricular> componentesCurricularesRetorno, IEnumerable<ComponenteCurricularTerritorioSaber> componentesTerritorio, IEnumerable<ComponenteCurricular> informacoesComponentes)
        {
    		var componentesConcatenados = componentesCurricularesRetorno.Where(a => !componentesTerritorio.Any(x => x.CodigoComponenteCurricular == a.Codigo)).ToList();
            return componentesConcatenados.Concat(componentesTerritorio.Select(ct => new ComponenteCurricular()
                                                                            {
                                                                                  Codigo = ct.CodigoComponenteCurricular,
                                                                                  CodigoComponenteCurricularTerritorioSaber = ct.CodigoComponenteCurricular,
                                                                                  Descricao = ct.ObterDescricaoComponenteCurricular(),
                                                                                  TerritorioSaber = true,
                                                                                  GrupoMatrizId = informacoesComponentes.Where(w => w.Codigo == ct.CodigoComponenteCurricular).FirstOrDefault().GrupoMatrizId,
                                                                                  Frequencia = true
                                                                            })).ToList();
        }

        public List<ComponenteCurricular> ConcatenarComponenteAgrupamentoTerritorio(IEnumerable<ComponenteCurricular> componentesCurricularesRetorno, IEnumerable<AgrupamentoAtribuicaoTerritorioSaber> componentesAgrupamentoTerritorio, IEnumerable<ComponenteCurricular> informacoesComponentes)
        {
            var componentesAgrupamento = componentesAgrupamentoTerritorio.Select(ct => new ComponenteCurricular()
            {
                Codigo = ct.CodigoAgrupamento,
                Descricao = ct.DescricaoAgrupamentoTerritorioSaber(),
                CodigoComponenteCurricularTerritorioSaber = ct.ComponentesCurricularesAgrupados.FirstOrDefault(),
                TerritorioSaber = true,
                GrupoMatrizId = informacoesComponentes.Where(w => w.Codigo == ct.ComponentesCurricularesAgrupados.FirstOrDefault()).FirstOrDefault().GrupoMatrizId,
                Frequencia = true
            }).DistinctBy(cc => cc.Codigo);
            return componentesCurricularesRetorno.Concat(componentesAgrupamento).ToList();
        }
        
    }
}
