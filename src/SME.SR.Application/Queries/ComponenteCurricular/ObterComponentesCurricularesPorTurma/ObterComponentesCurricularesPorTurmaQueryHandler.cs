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
    public class ObterComponentesCurricularesPorTurmaQueryHandler : IRequestHandler<ObterComponentesCurricularesPorTurmaQuery, IEnumerable<ComponenteCurricularPorTurma>>
    {
        private readonly IComponenteCurricularRepository componenteCurricularRepository;

        public ObterComponentesCurricularesPorTurmaQueryHandler(IComponenteCurricularRepository componenteCurricularRepository)
        {
            this.componenteCurricularRepository = componenteCurricularRepository ?? throw new ArgumentNullException(nameof(componenteCurricularRepository));
        }

        public async Task<IEnumerable<ComponenteCurricularPorTurma>> Handle(ObterComponentesCurricularesPorTurmaQuery request, CancellationToken cancellationToken)
        {
            var componentesDaTurma = await componenteCurricularRepository.ObterComponentesPorTurma(request.CodigoTurma);
            if (componentesDaTurma != null && componentesDaTurma.Any())
            {
                var componentesApiEol = await componenteCurricularRepository.ListarApiEol();
                var gruposMatriz = await componenteCurricularRepository.ListarGruposMatriz();

                List<ComponenteCurricularPorTurma> componentes = new List<ComponenteCurricularPorTurma>();

                foreach (var componente in componentesDaTurma)
                {
                    var cc = new ComponenteCurricularPorTurma
                    {
                        CodDisciplina = componente.Codigo,
                        CodDisciplinaPai = componente.CodigoComponentePai(componentesApiEol),
                        BaseNacional = componente.EhBaseNacional(componentesApiEol),
                        Compartilhada = componente.EhCompartilhada(componentesApiEol),
                        Disciplina = componente.DescricaoFormatada,
                        GrupoMatriz = componente.ObterGrupoMatriz(gruposMatriz),
                        LancaNota = componente.PodeLancarNota(componentesApiEol),
                        Regencia = componente.EhRegencia(componentesApiEol),
                        TerritorioSaber = componente.TerritorioSaber,
                        TipoEscola = componente.TipoEscola,
                        Frequencia = componentesApiEol.FirstOrDefault(x => x.IdComponenteCurricular == componente.Codigo) != null ? componentesApiEol.FirstOrDefault(x => x.IdComponenteCurricular == componente.Codigo).PermiteRegistroFrequencia : false
                    };
                    componentes.Add(cc);
                }
                return componentes;
            }

            return Enumerable.Empty<ComponenteCurricularPorTurma>();
        }
    }
}
