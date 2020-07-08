using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterNotasEFrequenciasDosAlunosQueryHandler : IRequestHandler<ObterNotasEFrequenciasDosAlunosQuery, IEnumerable<HistoricoEscolarDTO>>
    {
        public ObterNotasEFrequenciasDosAlunosQueryHandler()
        {
        }

        public Task<IEnumerable<HistoricoEscolarDTO>> Handle(ObterNotasEFrequenciasDosAlunosQuery request, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}
