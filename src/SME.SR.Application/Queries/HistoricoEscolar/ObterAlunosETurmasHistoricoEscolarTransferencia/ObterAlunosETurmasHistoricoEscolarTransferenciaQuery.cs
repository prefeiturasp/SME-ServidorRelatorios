using MediatR;
using SME.SR.Data;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterAlunosETurmasHistoricoEscolarTransferenciaQuery : IRequest<IEnumerable<AlunoTurmasHistoricoEscolarDto>>
    {
        public ObterAlunosETurmasHistoricoEscolarTransferenciaQuery(long codigoTurma, long[] codigoAlunos)
        {
            CodigoTurma = codigoTurma;
            CodigoAlunos = codigoAlunos;
        }

        public long CodigoTurma { get; set; }
        public long[] CodigoAlunos { get; set; }
    }
}
