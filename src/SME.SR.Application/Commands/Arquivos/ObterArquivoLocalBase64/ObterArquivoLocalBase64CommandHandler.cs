using MediatR;
using SME.SR.Infra;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterArquivoLocalBase64CommandHandler : IRequestHandler<ObterArquivoLocalBase64Command, string>
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public ObterArquivoLocalBase64CommandHandler(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<string> Handle(ObterArquivoLocalBase64Command request, CancellationToken cancellationToken)
        {
            var nomeArquivo =
                Path.Combine($"{variaveisAmbiente.PastaArquivosSGP ?? AppDomain.CurrentDomain.BaseDirectory}",
                    $"{request.CaminhoArquivo.Replace(@"/", @"\")}");
            if (!File.Exists(nomeArquivo))
                return "";

            // TODO usar o string mimeType = MimeMapping.GetMimeMapping(fileName); apos atualização para >= .Net 4.5
            string mimeType = MimeTypes.GetMimeType(nomeArquivo);

            return $"data:{mimeType};base64,{Convert.ToBase64String(File.ReadAllBytes(nomeArquivo))}";
        }
    }
}
