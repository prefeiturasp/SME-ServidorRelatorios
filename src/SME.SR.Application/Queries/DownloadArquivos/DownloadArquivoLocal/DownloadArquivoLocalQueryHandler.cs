using MediatR;
using Sentry;
using SME.SR.Infra;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application.Commands.RetornarRelatorioPronto
{
    public class DownloadArquivoLocalQueryHandler : IRequestHandler<DownloadArquivoLocalQuery, byte[]>
    {
        public DownloadArquivoLocalQueryHandler()        {
            
        }
        public async Task<byte[]> Handle(DownloadArquivoLocalQuery request, CancellationToken cancellationToken)
        {
            var caminhoBase = AppDomain.CurrentDomain.BaseDirectory;
            var nomeArquivo = $"{request.PastaFisicaCaminho}/{request.ArquivoNome}";
            var caminhoArquivo = Path.Combine($"{caminhoBase}", nomeArquivo);
            
            SentrySdk.AddBreadcrumb($"Caminho arquivo para download: {caminhoArquivo}");

            var arquivo = await File.ReadAllBytesAsync(caminhoArquivo);
            if (arquivo != null)
                return arquivo;

            throw new NegocioException("Não foi possível fazer download do arquivo");
        }
    }
}
