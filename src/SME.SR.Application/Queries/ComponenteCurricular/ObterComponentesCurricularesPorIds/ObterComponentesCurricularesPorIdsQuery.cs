using MediatR;
using SME.SR.Data;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterComponentesCurricularesPorIdsQuery : IRequest<IEnumerable<ComponenteCurricularPorTurma>>
    {
        public long[] ComponentesCurricularesIds { get; set; }
    }
}
