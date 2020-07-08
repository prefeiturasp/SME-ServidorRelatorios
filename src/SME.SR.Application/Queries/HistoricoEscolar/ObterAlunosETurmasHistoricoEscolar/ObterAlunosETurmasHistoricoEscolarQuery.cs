using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterAlunosETurmasHistoricoEscolarQuery : IRequest<IEnumerable<AlunoTurmasHistoricoEscolarDto>>
    {
        public ObterAlunosETurmasHistoricoEscolarQuery(long codigoTurma, long[] codigoAlunos)
        {
            CodigoTurma = codigoTurma;
            CodigoAlunos = codigoAlunos;
        }

        public long CodigoTurma { get; set; }
        public long[] CodigoAlunos { get; set; }
    }
}
