using MediatR;
using SME.SR.Data;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterAlunosPorTurmasRelatorioFrequenciaQuery : IRequest<IEnumerable<AlunoTurma>>
    {
        public IEnumerable<string> TurmasCodigos { get; set; }

        public ObterAlunosPorTurmasRelatorioFrequenciaQuery(IEnumerable<string> turmasCodigos)
        {
            TurmasCodigos = turmasCodigos;
        }
    }
}
