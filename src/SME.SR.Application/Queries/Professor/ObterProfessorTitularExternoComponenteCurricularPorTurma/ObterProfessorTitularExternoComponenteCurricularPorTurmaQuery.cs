using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterProfessorTitularExternoComponenteCurricularPorTurmaQuery : IRequest<IEnumerable<ProfessorTitularComponenteCurricularDto>>
    {
        public string[] CodigosTurma { get; set; }

        public ObterProfessorTitularExternoComponenteCurricularPorTurmaQuery(string[] codigosTurma)
        {
            CodigosTurma = codigosTurma;
        }
    }
}
