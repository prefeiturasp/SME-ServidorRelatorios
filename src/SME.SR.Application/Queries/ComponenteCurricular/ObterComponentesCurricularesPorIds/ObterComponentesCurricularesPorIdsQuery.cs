using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterComponentesCurricularesPorIdsQuery : IRequest<IEnumerable<DisciplinaDto>>
    {
        public ObterComponentesCurricularesPorIdsQuery(long[] ids)
        {
            Ids = ids;
        }
        public long[] Ids { get; set; }
    }
}
