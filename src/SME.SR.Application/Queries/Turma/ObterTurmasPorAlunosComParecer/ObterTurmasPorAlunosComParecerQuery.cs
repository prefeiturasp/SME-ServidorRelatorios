using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterTurmasPorAlunosComParecerQuery : IRequest<IEnumerable<AlunosTurmasCodigosDto>>
    {
        public ObterTurmasPorAlunosComParecerQuery(long[] alunosCodigos, long[] pareceresConclusivosIds)
        {
            AlunosCodigos = alunosCodigos;
            PareceresConclusivosIds = pareceresConclusivosIds;
        }

        public long[] AlunosCodigos { get; set; }
        public long[] PareceresConclusivosIds { get; set; }


    }
}
