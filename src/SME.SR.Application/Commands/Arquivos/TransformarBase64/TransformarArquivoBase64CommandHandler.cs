using MediatR;
using SME.SR.Infra;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class TransformarArquivoBase64CommandHandler : IRequestHandler<TransformarArquivoBase64Command, string>
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public TransformarArquivoBase64CommandHandler(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));

        }

        public async Task<string> Handle(TransformarArquivoBase64Command request, CancellationToken cancellationToken)
        {
            var diretorio = Path.Combine(variaveisAmbiente.PastaArquivosSGP ?? string.Empty,  $@"Arquivos\{request.Arquivo.Tipo}");

            if (!Directory.Exists(diretorio))
                Directory.CreateDirectory(diretorio);

            var nomeArquivo = $"{request.Arquivo.Codigo}.{request.Arquivo.Extensao}";
            var caminhoArquivo = Path.Combine(diretorio, nomeArquivo);
            if (!File.Exists(caminhoArquivo))
                return "";

            return $"data:{request.Arquivo.TipoArquivo};base64,{Convert.ToBase64String(File.ReadAllBytes(caminhoArquivo))}";
        }

    }
}
