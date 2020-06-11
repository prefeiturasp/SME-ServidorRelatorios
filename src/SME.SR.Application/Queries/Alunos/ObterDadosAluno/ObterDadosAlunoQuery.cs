using MediatR;
using SME.SR.Data;

namespace SME.SR.Application
{
    public class ObterDadosAlunoQuery : IRequest<Aluno>
    {
        public string CodigoTurma { get; set; }
        public string CodigoAluno { get; set; }
    }
}
