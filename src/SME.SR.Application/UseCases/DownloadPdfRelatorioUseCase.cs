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
            var caminhoBase = AppDomain.CurrentDomain.BaseDirectory;
            var nomeArquivo = $"relatorios/{codigoCorrelacao}.pdf";
           var caminhoArquivo= Path.Combine($"{caminhoBase}", nomeArquivo);

            var arquivo = await File.ReadAllBytesAsync(caminhoArquivo);
            if (arquivo != null)
                return arquivo;

            throw new NegocioException("Relatório não encontrado.");
        }
    }
}
