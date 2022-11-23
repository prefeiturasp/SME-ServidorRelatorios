using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterHistoricoNotasConselhoClassePorTurmaIdQuery : IRequest<IEnumerable<HistoricoAlteracaoNotasDto>>
    {
        public ObterHistoricoNotasConselhoClassePorTurmaIdQuery(long turmaId, long tipoCalendarioId, int[] bimestres , long[] componentes)
        {
            TurmaId = turmaId;
            this.TipoCalendarioId = tipoCalendarioId;
            Bimestres = bimestres;
            Componentes = componentes;
        }

        public long TurmaId { get; set; }
        public long TipoCalendarioId { get; set; }
        public int[] Bimestres { get; set; }
        public long[] Componentes { get; set; }
    }
}
