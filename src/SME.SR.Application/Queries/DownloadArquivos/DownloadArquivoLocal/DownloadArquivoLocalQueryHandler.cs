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
        private readonly IMediator mediator;

        public DownloadArquivoLocalQueryHandler(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<byte[]> Handle(DownloadArquivoLocalQuery request, CancellationToken cancellationToken)
        {
            var caminhoBase = AppDomain.CurrentDomain.BaseDirectory;
            var nomeArquivo = Path.Combine(request.PastaFisicaCaminho, request.ArquivoNome);
            var caminhoArquivo = Path.Combine($"{caminhoBase}", nomeArquivo);

            await mediator.Send(new SalvarLogViaRabbitCommand($"Caminho arquivo para download: {caminhoArquivo}", LogNivel.Informacao));

            var arquivo = await File.ReadAllBytesAsync(caminhoArquivo);

            if (arquivo != null)
                return arquivo;

            throw new NegocioException("Não foi possível fazer download do arquivo");
        }
    }
}