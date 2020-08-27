using MediatR;
using SME.SR.Data;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterDadosAlunosPorCodigosQuery : IRequest<IEnumerable<AlunoHistoricoEscolar>>
    {
        public ObterDadosAlunosPorCodigosQuery(long[] codigosAluno)
        {
            CodigosAluno = codigosAluno;
        }

        public long[] CodigosAluno { get; set; }
    }
}
