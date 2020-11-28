using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterHistoricoNotasConselhoClassePorTurmaIdQuery : IRequest<IEnumerable<HistoricoAlteracaoNotasDto>>
    {
        public ObterHistoricoNotasConselhoClassePorTurmaIdQuery(long turmaId)
        {
            TurmaId = turmaId;
        }

        public long TurmaId { get; set; }
    }
}
