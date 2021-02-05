using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterTurmasEModalidadesPorCodigoTurmasQuery : IRequest<IEnumerable<TurmaResumoDto>>
    {
        public ObterTurmasEModalidadesPorCodigoTurmasQuery(string[] turmasCodigos)
        {
            TurmasCodigos = turmasCodigos;
        }

        public string[] TurmasCodigos { get; set; }

    }
}
