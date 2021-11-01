using MediatR;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class SalvarArquivoFisicoCommandHandler : IRequestHandler<SalvarArquivoFisicoCommand, bool>
    {
        public SalvarArquivoFisicoCommandHandler()
        {

        }

        public async Task<bool> Handle(SalvarArquivoFisicoCommand request, CancellationToken cancellationToken)
        {
            var caminhoBase = AppDomain.CurrentDomain.BaseDirectory;
            var nomeArquivo = $"{request.Pasta}/{request.ArquivoNome}";
            var caminhoArquivo = Path.Combine($"{caminhoBase}", nomeArquivo);

            await File.WriteAllBytesAsync(caminhoArquivo, request.Arquivo);

            return true;
        }

    }
}
