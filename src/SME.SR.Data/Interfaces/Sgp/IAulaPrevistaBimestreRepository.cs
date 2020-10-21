using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public interface IAulaPrevistaBimestreRepository
    {
        Task<IEnumerable<AulaPrevistaBimestreQuantidade>> ObterBimestresAulasPrevistasPorFiltro(long tipoCalendarioId, string turmaId, string disciplinaId);

        Task<IEnumerable<AulaPrevistaBimestreQuantidade>> ObterBimestresAulasPrevistasPorId(long? aulaPrevistaId);
    }
}