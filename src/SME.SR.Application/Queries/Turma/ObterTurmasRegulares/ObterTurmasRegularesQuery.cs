using MediatR;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterTurmasRegularesQuery : IRequest<IEnumerable<string>>
    {
        public ObterTurmasRegularesQuery(string[] codigosAlunos)
        {
            CodigosAlunos = codigosAlunos;
        }

        public string[] CodigosAlunos { get; set; }
    }
}
