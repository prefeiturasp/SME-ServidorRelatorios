using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterTurmasPorAlunosQuery : IRequest<IEnumerable<AlunosTurmasCodigosDto>>
    {
        public ObterTurmasPorAlunosQuery(long[] alunosCodigos, long[] pareceresConclusivosIds = null)
        {
            AlunosCodigos = alunosCodigos;
            PareceresConclusivosIds = pareceresConclusivosIds;
        }

        public long[] AlunosCodigos { get; set; }
        public long[] PareceresConclusivosIds { get; set; }


    }
}
