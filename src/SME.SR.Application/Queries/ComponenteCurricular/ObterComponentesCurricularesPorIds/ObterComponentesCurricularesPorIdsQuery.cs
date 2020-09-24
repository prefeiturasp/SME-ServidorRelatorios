using MediatR;
using SME.SR.Data;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterComponentesCurricularesPorIdsQuery : IRequest<IEnumerable<ComponenteCurricularPorTurma>>
    {
        public ObterComponentesCurricularesPorIdsQuery()
        {

        }
        public ObterComponentesCurricularesPorIdsQuery(long id)
        {
            ComponentesCurricularesIds = new long[] { id };
        }
        public long[] ComponentesCurricularesIds { get; set; }
    }
}
