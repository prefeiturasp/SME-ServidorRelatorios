using MediatR;
using SME.SR.Data;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterDadosHistoricoAlunosPorCodigosQuery : IRequest<IEnumerable<AlunoHistoricoEscolar>>
    {
        public ObterDadosHistoricoAlunosPorCodigosQuery(long[] codigosAluno)
        {
            CodigosAluno = codigosAluno;
        }

        public long[] CodigosAluno { get; set; }
    }
}
