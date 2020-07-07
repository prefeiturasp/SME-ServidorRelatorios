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

        public Task<bool> Handle(SalvarArquivoFisicoCommand request, CancellationToken cancellationToken)
        {
            var caminhoBase = AppDomain.CurrentDomain.BaseDirectory;
            var nomeArquivo = $"{request.Pasta}/{request.ArquivoNome}";
            var caminhoArquivo = Path.Combine($"{caminhoBase}", nomeArquivo);

            File.WriteAllBytes(caminhoArquivo, request.Arquivo);

            return Task.FromResult(true);
        }

    }
}
