using MediatR;
using SME.SR.Data;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterAlunosPorTurmaNotasConceitosQuery : IRequest<IEnumerable<Aluno>>
    {
        public string TurmaCodigo { get; set; } 
    }
}
