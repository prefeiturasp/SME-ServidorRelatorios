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
    public class ObterComponentesCurricularesPorCodigoETurmaQueryHandler : IRequestHandler<ObterComponentesCurricularesPorCodigoETurmaQuery, IEnumerable<ComponenteCurricularPorTurma>>
    {
        private readonly IComponenteCurricularRepository componenteCurricularRepository;

        public ObterComponentesCurricularesPorCodigoETurmaQueryHandler(IComponenteCurricularRepository componenteCurricularRepository)
        {
            this.componenteCurricularRepository = componenteCurricularRepository ?? throw new ArgumentNullException(nameof(componenteCurricularRepository));
        }

        public async Task<IEnumerable<ComponenteCurricularPorTurma>> Handle(ObterComponentesCurricularesPorCodigoETurmaQuery request, CancellationToken cancellationToken)
        {
            var componentesDaTurma = await componenteCurricularRepository.ObterComponentesPorCodigoETurma(request.TurmaCodigo, request.ComponentesCodigo);
            if (componentesDaTurma != null && componentesDaTurma.Any())
            {
                var componentesApiEol = await componenteCurricularRepository.ListarApiEol();
                var gruposMatriz = await componenteCurricularRepository.ListarGruposMatriz();

                return componentesDaTurma?.Select(c => new ComponenteCurricularPorTurma
                {
                    CodDisciplina = c.Codigo,
                    CodDisciplinaPai = c.CodigoComponentePai(componentesApiEol),
                    BaseNacional = c.EhBaseNacional(componentesApiEol),
                    Compartilhada = c.EhCompartilhada(componentesApiEol),
                    Disciplina = c.DescricaoFormatada,
                    GrupoMatriz = c.ObterGrupoMatriz(gruposMatriz),
                    LancaNota = c.PodeLancarNota(componentesApiEol),
                    Regencia = c.EhRegencia(componentesApiEol),
                    TerritorioSaber = c.TerritorioSaber,
                    TipoEscola = c.TipoEscola
                });
            }
            return Enumerable.Empty<ComponenteCurricularPorTurma>();
        }

    }
}
