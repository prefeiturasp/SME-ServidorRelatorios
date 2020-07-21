using MediatR;
using SME.SR.Data;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterFechamentosPorCodigosTurmaQuery : IRequest<IEnumerable<FechamentoTurma>>
    {
        public string[] CodigosTurma { get; set; }
    }
}
