using System.Threading.Tasks;

namespace SME.SR.Data.Interfaces
{
    public interface IFechamentoTurmaRepository
    {
        Task<FechamentoTurma> ObterTurmaPeriodoFechamentoPorId(long fechamentoTurmaId);
    }
}
