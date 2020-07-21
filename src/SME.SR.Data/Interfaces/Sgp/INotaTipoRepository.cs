using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public interface INotaTipoRepository
    {
        Task<string> ObterPorCicloIdDataAvalicacao(long? cicloId, DateTime dataReferencia);

        Task<IEnumerable<TipoNotaCicloAno>> ObterPorCiclosIdDataAvalicacao(long[] ciclosId, DateTime dataReferencia);

        Task<IEnumerable<TipoNotaCicloAno>> Listar();
    }
}
