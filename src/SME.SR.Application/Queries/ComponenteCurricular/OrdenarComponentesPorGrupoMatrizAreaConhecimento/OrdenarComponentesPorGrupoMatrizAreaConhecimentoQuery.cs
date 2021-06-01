using MediatR;
using SME.SR.Data;
using System.Collections.Generic;

namespace SME.SR.Application
{
  public  class OrdenarComponentesPorGrupoMatrizAreaConhecimentoQuery : IRequest<IEnumerable<ComponenteCurricularPorTurma>>
    {
        public OrdenarComponentesPorGrupoMatrizAreaConhecimentoQuery(IEnumerable<ComponenteCurricularPorTurma> componentesCurriculares)
        {
            ComponentesCurriculares = componentesCurriculares;
        }

        public IEnumerable<ComponenteCurricularPorTurma> ComponentesCurriculares { get; set; }
    }
}
