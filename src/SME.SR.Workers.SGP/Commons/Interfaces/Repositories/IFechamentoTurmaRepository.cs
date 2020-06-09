using SME.SR.Workers.SGP.Models;
using System.Threading.Tasks;

namespace SME.SR.Workers.SGP.Commons
{
    public interface IFechamentoTurmaRepository
    {
        Task<FechamentoTurma> ObterTurmaPeriodoFechamentoPorId(long fechamentoTurmaId);
    }
}
