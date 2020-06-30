using MediatR;
using SME.SR.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Application.Queries.ComponenteCurricular.ObterComponentesCurricularesPorIds
{
    public class ObterComponentesCurricularesPorIdsQuery : IRequest<IEnumerable<ComponenteCurricularPorTurma>>
    {
        public long[] ComponentesCurricularesIds { get; set; }
    }
}
