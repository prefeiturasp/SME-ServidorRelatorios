using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application.Queries
{
    public class ObterHistoricoNotasConselhoClasseQuery : IRequest<IEnumerable<HistoricoAlteracaoNotasDto>>
    {
        public ObterHistoricoNotasConselhoClasseQuery(long turmaId)
        {
            TurmaId = turmaId;
        }

        public long TurmaId { get; set; }
    }
}
