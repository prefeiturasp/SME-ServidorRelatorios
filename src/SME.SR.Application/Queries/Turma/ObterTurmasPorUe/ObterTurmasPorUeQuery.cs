using MediatR;
using SME.SR.Data;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterTurmasPorUeQuery : IRequest<IEnumerable<Turma>>
    {
        public string CodigoUe { get; set; }
    }
}
