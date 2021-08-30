using MediatR;
using SME.SR.Data;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public  class ObterTurmasPorCodigoQuery : IRequest<IEnumerable<Turma>>
    {
        public ObterTurmasPorCodigoQuery(string[] codigos)
        {
            Codigos = codigos;
        }

        public string[] Codigos { get; set; }
    }
}
