using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public interface INotaTipoRepository
    {
        Task<string> ObterPorCicloIdDataAvalicacao(long? cicloId, DateTime dataReferencia);
    }
}
