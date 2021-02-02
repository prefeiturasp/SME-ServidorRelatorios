using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterProfessorTitularComponenteCurricularPorCodigosRfQuery : IRequest<IEnumerable<ProfessorTitularComponenteCurricularDto>>
    {
        public string[] CodigosRf { get; set; }

        public ObterProfessorTitularComponenteCurricularPorCodigosRfQuery(string[] codigosRf)
        {
            CodigosRf = codigosRf;
        }
    }
}
