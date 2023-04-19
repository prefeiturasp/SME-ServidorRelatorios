using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
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
            var lstComponentes = await componenteCurricularRepository.ListarComponentes();
        
            lstComponentes = lstComponentes
                   .Where(w => request.ComponentesCurricularesIds.Contains(w.CodigoTerritorioSaber))
                   .ToList();

            var componenteIds = request.ComponentesCurricularesIds.Select(x => x.ToString()).ToArray();
            var componentesTS = lstComponentes.Where(c => c.TerritorioSaber)
                                .Select(c => c.CodigoTerritorioSaber.ToString()).ToArray();

            IEnumerable<ComponenteCurricular> componentesTerritorioSaber = null;

            if (ExisteComponentesTerritorioSaberTurma(componentesTS, request.TurmasId))
                componentesTerritorioSaber = request.TurmasId.Any() ? await componenteCurricularRepository.ListarComponentesTerritorioSaber(componentesTS, request.TurmasId) : null;

            if (componentesTerritorioSaber != null)
                lstComponentes = ConcatenarComponenteTerritorio(lstComponentes, componentesTerritorioSaber);

            if (lstComponentes != null && lstComponentes.Any())
            {
                var gruposMatriz = await componenteCurricularRepository.ListarGruposMatriz();
                var areasDoConhecimento = await areaDoConhecimentoRepository.ObterAreasDoConhecimentoPorComponentesCurriculares(componenteIds.Select(long.Parse).ToArray());

                return lstComponentes.Select(x => new ComponenteCurricularPorTurma
                {
                    CodDisciplina = x.CdComponenteCurricular,
                    CodigoTerritorioSaber = x.CodigoTerritorioSaber,
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

        private bool ExisteComponentesTerritorioSaberTurma(string[] componentesTS, string[] turmasId)
        {
            return componentesTS != null && componentesTS.Length > 0 && turmasId != null && turmasId.Length > 0;
        }

        public List<ComponenteCurricular> ConcatenarComponenteTerritorio(IEnumerable<ComponenteCurricular> componentes, IEnumerable<ComponenteCurricular> componentesTerritorio)
        {
		var componentesConcatenados = componentes.Where(a => !componentesTerritorio.Any(x => x.CodigoTerritorioSaber == a.CodigoTerritorioSaber)).ToList();
		return componentesConcatenados.Concat(componentesTerritorio).ToList();
        }
    }
}
