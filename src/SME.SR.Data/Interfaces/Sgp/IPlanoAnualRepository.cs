using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Data.Interfaces
{
    public interface IPlanoAnualRepository
    {
        Task<PlanoAnualDto> ObterPorId(long Id);
        Task<IEnumerable<BimestreDescricaoObjetivosPlanoAnualDto>> ObterObjetivosPorPlanoAulaId(long Id);
    }
}

