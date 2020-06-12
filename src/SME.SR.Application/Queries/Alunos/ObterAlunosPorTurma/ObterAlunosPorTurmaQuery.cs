using MediatR;
using SME.SR.Data;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterAlunosPorTurmaQuery : IRequest<IEnumerable<Aluno>>
    {
        public string CodigoTurma { get; set; } 
    }
}
