using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using SME.SR.Infra.Dtos.ElasticSearch;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterAlunosMatriculasPorTurmasQuery : IRequest<IEnumerable<AlunoNaTurmaDTO>>
    {
        public ObterAlunosMatriculasPorTurmasQuery(int[] codigosTurmas)
        {
            CodigosTurmas = codigosTurmas;
        }

        public int[] CodigosTurmas { get; set; }
    }
}
