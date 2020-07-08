using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterNotasEFrequenciasDosAlunosQuery : IRequest<IEnumerable<AlunoTurmasNotasFrequenciasDto>>
    {
        public ObterNotasEFrequenciasDosAlunosQuery(long codigoTurma, long[] codigoAlunos)
        {
            CodigoTurma = codigoTurma;
            CodigoAlunos = codigoAlunos;
        }

        public long CodigoTurma { get; set; }
        public long[] CodigoAlunos { get; set; }
    }
}
