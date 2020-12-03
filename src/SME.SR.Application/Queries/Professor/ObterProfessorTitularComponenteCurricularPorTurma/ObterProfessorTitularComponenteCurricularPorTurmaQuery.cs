using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterProfessorTitularComponenteCurricularPorTurmaQuery : IRequest<IEnumerable<ProfessorTitularComponenteCurricularDto>>
    {
        public string[] CodigosTurma { get; set; }
    }
}
