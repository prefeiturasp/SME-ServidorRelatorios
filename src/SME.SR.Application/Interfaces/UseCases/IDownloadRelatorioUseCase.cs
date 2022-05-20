using System;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public interface IDownloadRelatorioUseCase
    {
        Task<byte[]> Executar(Guid codigoCorrelacao, string extensao, string diretorio);
    }
}