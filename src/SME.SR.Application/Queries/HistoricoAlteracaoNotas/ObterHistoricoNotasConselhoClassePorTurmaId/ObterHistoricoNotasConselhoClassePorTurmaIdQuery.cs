using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterHistoricoNotasConselhoClassePorTurmaIdQuery : IRequest<IEnumerable<HistoricoAlteracaoNotasDto>>
    {
        public ObterHistoricoNotasConselhoClassePorTurmaIdQuery(long turmaId, long tipoCalendarioId)
        {
            TurmaId = turmaId;
            this.TipoCalendarioId = tipoCalendarioId;
        }

        public long TurmaId { get; set; }
        public long TipoCalendarioId { get; set; }
    }
}
