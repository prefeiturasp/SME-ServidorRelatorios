using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;
using System.Linq;

namespace SME.SR.Application
{
    public class ObterUesConclusaoQuery : IRequest<IEnumerable<IGrouping<long, UeConclusaoPorAlunoAno>>>
    {
        public long[] CodigosAlunos { get; internal set; }
        public Modalidade Modalidade { get; internal set; }
    }
}
