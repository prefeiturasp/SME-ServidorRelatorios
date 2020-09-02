using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterAlunosPorTurmaSemParecerConclusivoQuery : IRequest<IEnumerable<AlunosTurmasCodigosDto>>
    {
        public ObterAlunosPorTurmaSemParecerConclusivoQuery(long turmaCodigo)
        {
            TurmaCodigo = turmaCodigo;
        }

        public long TurmaCodigo { get; set; }
    }
}
