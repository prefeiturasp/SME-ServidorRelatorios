using MediatR;
using SME.SR.Data;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterAreasConhecimentoComponenteCurricularQuery : IRequest<IEnumerable<AreaDoConhecimento>>
    {
        public ObterAreasConhecimentoComponenteCurricularQuery(string[] codigosComponenteCurricular)
        {
            CodigosComponenteCurricular = codigosComponenteCurricular;
        }

        public string[] CodigosComponenteCurricular { get; set; }
    }
}
