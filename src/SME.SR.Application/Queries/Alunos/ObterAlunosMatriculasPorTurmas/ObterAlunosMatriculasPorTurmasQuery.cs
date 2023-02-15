using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterAlunosMatriculasPorTurmasQuery : IRequest<IEnumerable<AlunoTurma>>
    {
        public ObterAlunosMatriculasPorTurmasQuery(int[] codigosTurmas)
        {
            CodigosTurmas = codigosTurmas;
        }

        public int[] CodigosTurmas { get; set; }
    }
}
