using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data.Interfaces
{
    public interface IArquivoRepository
    {
        Task<ArquivoDto> ObterPorCodigo(Guid codigo);
    }
}