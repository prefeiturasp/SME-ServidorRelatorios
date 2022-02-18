using SME.SR.Infra;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data.Interfaces
{
    public interface IDevolutivaRepository
    {
        Task<IEnumerable<DevolutivaDto>> ObterDevolutivas(long ueId, IEnumerable<long> turmas, IEnumerable<int> bimestres, int ano, long componenteCurricular, bool utilizarLayoutNovo);
        Task<DevolutivaSincronoDto> ObterDevolutiva(long devolutivaId);
    }
}
