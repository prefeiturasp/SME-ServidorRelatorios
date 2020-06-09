using MediatR;
using SME.SR.Workers.SGP.Models;
using System.Collections.Generic;

namespace SME.SR.Workers.SGP.Queries
{
    public class ObterAlunosPorTurmaQuery : IRequest<IEnumerable<Aluno>>
    {
        public string CodigoTurma { get; set; } 
    }
}
