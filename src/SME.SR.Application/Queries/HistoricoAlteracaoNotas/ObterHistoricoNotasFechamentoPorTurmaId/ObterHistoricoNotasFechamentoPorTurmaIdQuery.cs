using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterHistoricoNotasFechamentoPorTurmaIdQuery : IRequest<IEnumerable<HistoricoAlteracaoNotasDto>>
    {
        public ObterHistoricoNotasFechamentoPorTurmaIdQuery(long turmaId, long tipocalendarioId)
        {
            TurmaId = turmaId;
            this.tipocalendarioId = tipocalendarioId;
        }

        public long TurmaId { get; set; }
        public long tipocalendarioId { get; set; }

    }
}
