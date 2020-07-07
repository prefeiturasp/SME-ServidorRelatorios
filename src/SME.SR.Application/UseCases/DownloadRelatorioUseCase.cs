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
            this.mediator = mediator;
        }
        public async Task<byte[]> Executar(Guid codigoCorrelacao, string extensao)
        {
            return await mediator.Send(new DownloadArquivoLocalQuery(codigoCorrelacao.ToString() + extensao, "relatorios"));
        }
    }
}
