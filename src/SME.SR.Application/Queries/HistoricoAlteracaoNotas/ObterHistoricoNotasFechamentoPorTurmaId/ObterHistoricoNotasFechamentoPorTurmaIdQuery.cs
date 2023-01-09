using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterHistoricoNotasFechamentoPorTurmaIdQuery : IRequest<IEnumerable<HistoricoAlteracaoNotasDto>>
    {
        public ObterHistoricoNotasFechamentoPorTurmaIdQuery(long turmaId, long tipocalendarioId, int[] bimestres, long[] componentes)
        {
            TurmaId = turmaId;
            this.tipocalendarioId = tipocalendarioId;
            Bimestres = bimestres;
            Componentes = componentes;
        }

        public long TurmaId { get; set; }
        public long tipocalendarioId { get; set; }
        public int[] Bimestres { get; set; }
        public long[] Componentes { get; set; }
    }
}
