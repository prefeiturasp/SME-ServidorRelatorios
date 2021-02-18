using MediatR;
using SME.SR.Data;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterComponentesCurricularesEolPorIdsQuery : IRequest<IEnumerable<ComponenteCurricularPorTurma>>
    {
        public ObterComponentesCurricularesEolPorIdsQuery()
        {

        }
        public ObterComponentesCurricularesEolPorIdsQuery(long id)
        {
            ComponentesCurricularesIds = new long[] { id };
        }

        public ObterComponentesCurricularesEolPorIdsQuery(long[] ids)
        {
            ComponentesCurricularesIds = ids;
        }

        public long[] ComponentesCurricularesIds { get; set; }
    }
}
