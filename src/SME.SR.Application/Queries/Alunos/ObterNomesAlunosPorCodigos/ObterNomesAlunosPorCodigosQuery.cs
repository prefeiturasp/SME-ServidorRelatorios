using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterNomesAlunosPorCodigosQuery : IRequest<IEnumerable<AlunoNomeDto>>
    {
        public ObterNomesAlunosPorCodigosQuery(string[] codigos)
        {
            Codigos = codigos;
        }

        public string[] Codigos { get; }
    }
}
