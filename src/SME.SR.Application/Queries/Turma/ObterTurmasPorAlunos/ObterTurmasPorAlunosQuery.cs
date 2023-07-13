using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterTurmasPorAlunosQuery : IRequest<IEnumerable<AlunosTurmasCodigosDto>>
    {
        public ObterTurmasPorAlunosQuery(long[] alunosCodigos, int? anoLetivo = null)
        {
            AlunosCodigos = alunosCodigos;
            AnoLetivo = anoLetivo;
        }

        public long[] AlunosCodigos { get; set; }
        public int? AnoLetivo { get; set; }


    }
}
