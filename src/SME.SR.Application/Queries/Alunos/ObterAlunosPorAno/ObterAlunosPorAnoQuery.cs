using MediatR;
using SME.SR.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Application
{
    public class ObterAlunosPorAnoQuery: IRequest<IEnumerable<AlunoTurma>>
    {
        public ObterAlunosPorAnoQuery(IEnumerable<string> turmasCodigos)
        {
            TurmasCodigos = turmasCodigos;
        }

        public IEnumerable<string> TurmasCodigos { get; set; }
    }
}
