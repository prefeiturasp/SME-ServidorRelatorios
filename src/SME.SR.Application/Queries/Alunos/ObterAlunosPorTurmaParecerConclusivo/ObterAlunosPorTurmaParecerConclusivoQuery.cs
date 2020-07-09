using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterAlunosPorTurmaParecerConclusivoQuery : IRequest<IEnumerable<AlunosTurmasCodigosDto>>
    {
        public ObterAlunosPorTurmaParecerConclusivoQuery(long turmaCodigo, long[] pareceresConclusivosIds)
        {
            TurmaCodigo = turmaCodigo;
            PareceresConclusivosIds = pareceresConclusivosIds;
        }

        public long TurmaCodigo { get; set; }
        public long[] PareceresConclusivosIds { get; set; }

    }
}
