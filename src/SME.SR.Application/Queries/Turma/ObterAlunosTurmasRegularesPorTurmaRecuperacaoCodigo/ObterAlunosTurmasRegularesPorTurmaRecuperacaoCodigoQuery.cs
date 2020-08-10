using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterAlunosTurmasRegularesPorTurmaRecuperacaoCodigoQuery : IRequest<IEnumerable<AlunoTurmaRegularRetornoDto>>
    {
        public ObterAlunosTurmasRegularesPorTurmaRecuperacaoCodigoQuery(long turmaCodigo)
        {
            TurmaCodigo = turmaCodigo;
        }

        public long TurmaCodigo { get; set; }
    }
}
