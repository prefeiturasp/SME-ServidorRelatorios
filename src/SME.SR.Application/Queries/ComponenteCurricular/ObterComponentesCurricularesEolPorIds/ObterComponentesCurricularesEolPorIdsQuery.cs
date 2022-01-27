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

        public ObterComponentesCurricularesEolPorIdsQuery(long[] ids, string[] turmasId = null)
        {
            ComponentesCurricularesIds = ids;
            TurmasId = turmasId == null ? new string[] { } : turmasId;
        }

        public long[] ComponentesCurricularesIds { get; set; }
        public string[] TurmasId { get; set; }
    }
}
