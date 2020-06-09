using MediatR;
using SME.SR.Workers.SGP.Commons;
using SME.SR.Workers.SGP.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Workers.SGP.Queries
{
    public class ObterComponentesCurricularesPorTurmaQueryHandler : IRequestHandler<ObterComponentesCurricularesPorTurmaQuery, IEnumerable<ComponenteCurricularPorTurma>>
    {
        private IComponenteCurricularRepository _componenteCurricularRepository;

        public ObterComponentesCurricularesPorTurmaQueryHandler(IComponenteCurricularRepository componenteCurricularRepository)
        {
            this._componenteCurricularRepository = componenteCurricularRepository;
        }

        public async Task<IEnumerable<ComponenteCurricularPorTurma>> Handle(ObterComponentesCurricularesPorTurmaQuery request, CancellationToken cancellationToken)
        {
            var componentesDaTurma = await _componenteCurricularRepository.ObterComponentesPorTurma(request.CodigoTurma);
            if (componentesDaTurma != null && componentesDaTurma.Any())
            {
                var componentesApiEol = await _componenteCurricularRepository.Listar();
                var gruposMatriz = await _componenteCurricularRepository.ListarGruposMatriz();

                return componentesDaTurma?.Select(c => new ComponenteCurricularPorTurma
                {
                    CodDisciplina = c.Codigo,
                    CodDisciplinaPai = c.CodigoComponentePai(componentesApiEol),
                    BaseNacional = c.EhBaseNacional(componentesApiEol),
                    Compartilhada = c.EhCompartilhada(componentesApiEol),
                    Disciplina = c.DescricaoFormatada,
                    GrupoMatriz = c.ObterGrupoMatriz(componentesApiEol, gruposMatriz),
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
