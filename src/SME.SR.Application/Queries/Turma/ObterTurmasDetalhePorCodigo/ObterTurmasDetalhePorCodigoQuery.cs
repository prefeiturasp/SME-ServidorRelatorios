using MediatR;
using SME.SR.Data;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterTurmasDetalhePorCodigoQuery : IRequest<IEnumerable<Turma>>
    {
        public ObterTurmasDetalhePorCodigoQuery(long[] turmasCodigo)
        {
            TurmasCodigo = turmasCodigo;
        }

        public long[] TurmasCodigo { get; set; }
    }
}
