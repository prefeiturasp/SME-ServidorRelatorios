using MediatR;
using System.Collections.Generic;
using SME.SR.Infra;

namespace SME.SR.Application.Queries
{
    public class BuscarTurmasSrmERegularDoAlunoQuery : IRequest<IEnumerable<TurmasDoAlunoDTO>>
    {
        public BuscarTurmasSrmERegularDoAlunoQuery(long codigoAluno)
        {
            CodigoAluno = codigoAluno;
        }

        public long CodigoAluno { get; }
    }
}
