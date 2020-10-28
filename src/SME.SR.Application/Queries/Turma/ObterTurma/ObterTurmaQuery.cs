using MediatR;
using SME.SR.Data;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterTurmaQuery : IRequest<Turma>
    {
        public ObterTurmaQuery() { }
        public ObterTurmaQuery(string codigoTurma)
        {
            CodigoTurma = codigoTurma;
        }

        public string CodigoTurma { get; set; }
    }
}
