using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public  class ObterTurmasPorAlunosSemParecerQuery : IRequest<IEnumerable<AlunosTurmasCodigosDto>>
    {
        public ObterTurmasPorAlunosSemParecerQuery(long[] alunosCodigos)
        {
            AlunosCodigos = alunosCodigos;
        }

        public long[] AlunosCodigos { get; set; }
    }
}
