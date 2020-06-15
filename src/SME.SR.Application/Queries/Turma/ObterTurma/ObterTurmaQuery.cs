using MediatR;
using SME.SR.Data;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterTurmaQuery : IRequest<Turma>
    {
        public string CodigoTurma { get; set; }
    }
}
