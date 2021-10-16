using MediatR;
using SME.SR.Infra;
using SME.SR.JRSClient.Extensions;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class SalvarRelatorioJasperLocalCommandHandler : IRequestHandler<SalvarRelatorioJasperLocalCommand, bool>
    {
        private readonly JasperCookieHandler jasperCookieHandler;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IMediator mediator;

        public SalvarRelatorioJasperLocalCommandHandler(JasperCookieHandler jasperCookieHandler, IHttpClientFactory httpClientFactory, IMediator mediator)
        {
            this.jasperCookieHandler = jasperCookieHandler ?? throw new System.ArgumentNullException(nameof(jasperCookieHandler));
            this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        public async Task<bool> Handle(SalvarRelatorioJasperLocalCommand request, CancellationToken cancellationToken)
        {
            using var httpClient = httpClientFactory.CreateClient("jasperServer");

            if (!string.IsNullOrWhiteSpace(request.JSessionId))
                jasperCookieHandler.CookieContainer.Add(httpClient.BaseAddress, new System.Net.Cookie("JSESSIONID", request.JSessionId));

            var resposta = await httpClient.GetAsync($"/jasperserver/rest_v2/reportExecutions/{request.RequestId}/exports/{request.ExportID}/outputResource");

            if (resposta.IsSuccessStatusCode)
            {
                var relatorioBytes = await resposta.Content.ReadAsByteArrayAsync();

                return await mediator.Send(new SalvarArquivoFisicoCommand(relatorioBytes, "relatorios", $"{request.CorrelacaoId}.pdf"));
                 
            }
            else throw new NegocioException($"Não foi possível fazer o download do relatório. {request.CorrelacaoId}");
        }
        
    }
}
