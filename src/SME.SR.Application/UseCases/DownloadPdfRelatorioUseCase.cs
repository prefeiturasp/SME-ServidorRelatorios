using SME.SR.Infra;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class DownloadPdfRelatorioUseCase : IDownloadPdfRelatorioUseCase
    {
        public async Task<byte[]> Executar(Guid codigoCorrelacao)
        {
            var arquivo = await File.ReadAllBytesAsync("Relatorios/ConselhoAta.pdf");
            if (arquivo != null)
                return arquivo;

            throw new NegocioException("Relatório não encontrado.");
        }
    }
}
