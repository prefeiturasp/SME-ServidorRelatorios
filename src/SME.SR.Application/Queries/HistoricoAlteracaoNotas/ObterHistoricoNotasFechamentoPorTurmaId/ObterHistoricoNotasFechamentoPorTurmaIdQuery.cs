using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterHistoricoNotasFechamentoPorTurmaIdQuery : IRequest<IEnumerable<HistoricoAlteracaoNotasFechamentoDto>>
    {
        public ObterHistoricoNotasFechamentoPorTurmaIdQuery(long turmaId)
        {
            TurmaId = turmaId;
        }

        public long TurmaId { get; set; }
    }
}
