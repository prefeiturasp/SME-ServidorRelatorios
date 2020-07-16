using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data.Interfaces
{
    public interface IFechamentoTurmaRepository
    {
        Task<FechamentoTurma> ObterTurmaPeriodoFechamentoPorId(long fechamentoTurmaId);

        Task<IEnumerable<FechamentoTurma>> ObterFechamentosPorCodigosTurma(string[] codigoTurma);
    }
}
