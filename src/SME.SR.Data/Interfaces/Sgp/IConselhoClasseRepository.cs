using SME.SR.Infra;
using System.Threading.Tasks;

namespace SME.SR.Data.Interfaces
{
    public interface IConselhoClasseRepository
    {
        Task<long> ObterConselhoPorFechamentoTurmaId(long fechamentoTurmaId);
    }
}
