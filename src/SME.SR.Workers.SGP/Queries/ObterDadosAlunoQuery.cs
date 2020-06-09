using MediatR;
using SME.SR.Workers.SGP.Models;

namespace SME.SR.Workers.SGP.Queries
{
    public class ObterDadosAlunoQuery : IRequest<Aluno>
    {
        public string CodigoTurma { get; set; }
        public string CodigoAluno { get; set; }
    }
}
