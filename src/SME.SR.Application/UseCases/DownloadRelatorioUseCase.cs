using MediatR;
using System;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class DownloadRelatorioUseCase : IDownloadRelatorioUseCase
    {
        private readonly IMediator mediator;

        public DownloadRelatorioUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<byte[]> Executar(Guid codigoCorrelacao, string extensao, string diretorio)
        {
            return await mediator.Send(new DownloadArquivoLocalQuery(codigoCorrelacao.ToString() + extensao, diretorio));
        }

        public Task<string> ObterDiretorioComplementar(string relatorioNome)
        {
            switch (relatorioNome)
            {
                case "Itiner%C3%A2ncias.pdf":
                case "Itinerâncias.pdf":
                    return Task.FromResult("itinerancia");                
                case "Devolutivas.pdf":
                    return Task.FromResult("devolutiva");
                default:
                    return Task.FromResult(string.Empty);
            }
        }
    }
}
