using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterNomeAlunoPorCodigoQuery : IRequest<AlunoNomeDto>
    {
        public ObterNomeAlunoPorCodigoQuery(string codigo)
        {
            Codigo = codigo;
        }

        public string Codigo { get; }
    }
}
