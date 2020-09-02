using MediatR;
using SME.SR.Data;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterDadosAlunosHistoricoEscolarQuery : IRequest<IEnumerable<AlunoHistoricoEscolar>>
    {
        public long[] CodigosAluno { get; set; }
    }
}
