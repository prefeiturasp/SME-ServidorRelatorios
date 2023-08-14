using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Data.Interfaces
{
    public interface IPlanoAnualRepository
    {
        Task<IEnumerable<PlanoAnualBimestreObjetivosDto>> ObterPorId(long Id);
    }
}

