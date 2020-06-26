using System;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public interface IDownloadPdfRelatorioUseCase
    {
        Task<byte[]> Executar(Guid codigoCorrelacao);
    }
}