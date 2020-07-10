using MediatR;
using SME.SR.Data;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterAreasConhecimentoComponenteCurricularQuery : IRequest<IEnumerable<AreaDoConhecimento>>
    {
        public ObterAreasConhecimentoComponenteCurricularQuery(long[] codigosComponenteCurricular)
        {
            CodigosComponenteCurricular = codigosComponenteCurricular;
        }

        public long[] CodigosComponenteCurricular { get; set; }
    }
}
