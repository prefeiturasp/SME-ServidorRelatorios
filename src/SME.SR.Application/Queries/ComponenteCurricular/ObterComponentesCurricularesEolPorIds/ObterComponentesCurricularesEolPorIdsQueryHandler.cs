﻿using MediatR;
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
                   .Where(w => request.ComponentesCurricularesIds.Contains(w.Codigo))
                   .ToList();

            var componenteIds = request.ComponentesCurricularesIds.Select(x => x.ToString()).ToArray();

            lstComponentes = lstComponentes.Concat(await componenteCurricularRepository.ListarComponentesTerritorioSaber(componenteIds));

            if (lstComponentes != null && lstComponentes.Any())
            {
                var gruposMatriz = await componenteCurricularRepository.ListarGruposMatriz();
                var areasDoConhecimento = await areaDoConhecimentoRepository.ObterAreasDoConhecimentoPorComponentesCurriculares(componenteIds.Select(long.Parse).ToArray());

                return lstComponentes.Select(x => new ComponenteCurricularPorTurma
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
                    AreaDoConhecimento = x.ObterAreaDoConhecimento(areasDoConhecimento)
                });
            }

            return Enumerable.Empty<ComponenteCurricularPorTurma>();
        }

    }
}
