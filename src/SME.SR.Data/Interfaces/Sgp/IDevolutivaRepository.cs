using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Data.Interfaces
{
    public interface IDevolutivaRepository
    {
        Task<IEnumerable<DevolutivaDto>> ObterDevolutivas(long ueId, IEnumerable<long> turmas, IEnumerable<int> bimestres);
    }
}
