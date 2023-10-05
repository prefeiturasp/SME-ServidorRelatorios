using MediatR;
using SME.SR.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Application
{
    public class ObterTurmasComplementaresPorAlunosQuery : IRequest<IEnumerable<Turma>>
    {
        public ObterTurmasComplementaresPorAlunosQuery(string[] alunosCodigos)
        {
            AlunosCodigos = alunosCodigos;
        }
        public string[] AlunosCodigos { get; set; }
    }
}
