using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Application
{
    public class ObterGrupoMatrizIdPorComponenteCurricularIdQuery : IRequest<long>
    {
        public long ComponenteCurricularId { get; set; }

        public ObterGrupoMatrizIdPorComponenteCurricularIdQuery(long componenteCurricularId)
        {
            ComponenteCurricularId = componenteCurricularId;
        }
    }
}
