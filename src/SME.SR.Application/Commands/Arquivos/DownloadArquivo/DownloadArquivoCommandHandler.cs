using MediatR;
using SME.SR.Infra;
using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class DownloadArquivoCommandHandler : IRequestHandler<DownloadArquivoCommand, byte[]>
    {
        private readonly IServicoArmazenamento servicoArmazenamento;
        public DownloadArquivoCommandHandler(IServicoArmazenamento servicoArmazenamento)
        {
            this.servicoArmazenamento = servicoArmazenamento ?? throw new ArgumentNullException(nameof(servicoArmazenamento));
        }
        public async Task<byte[]> Handle(DownloadArquivoCommand request, CancellationToken cancellationToken)
        {
            var extensao = Path.GetExtension(request.Nome);

            var nomeArquivoComExtensao = $"{request.Codigo}{extensao}";

            var enderecoArquivo = servicoArmazenamento.Obter(nomeArquivoComExtensao);

            if (!string.IsNullOrEmpty(enderecoArquivo))
            {
                var response = await new HttpClient().GetAsync(enderecoArquivo);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    return await response.Content.ReadAsByteArrayAsync();

                return default;
            }
            throw new NegocioException("O arquivo não foi encontrada.");
        }
    }
}
