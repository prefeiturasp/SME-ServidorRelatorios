using MediatR;
using Newtonsoft.Json;
using SME.SR.Infra.Dtos.SondagemTurmaEscritaEF;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application.Queries.ConsultaSondagemPorTurma
{
    public class ConsultaSondagemPorTurmaQueryHandler : IRequestHandler<ConsultaSondagemPorTurmaQuery, ConsultaSondagemPorTurmaDto>
    {
        private readonly IHttpClientFactory httpClientFactory;

        public ConsultaSondagemPorTurmaQueryHandler(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        public async Task<ConsultaSondagemPorTurmaDto> Handle(ConsultaSondagemPorTurmaQuery request, CancellationToken cancellationToken)
        {
            var dto = new ConsultaSondagemPorTurmaDto();

            var httpClient = httpClientFactory.CreateClient("apiNovaSondagem");
            var url = MontarUrl(request);

            var resposta = await httpClient.GetAsync(url);
            if (resposta.IsSuccessStatusCode)
            {
                var json = await resposta.Content.ReadAsStringAsync();
                dto = JsonConvert.DeserializeObject<ConsultaSondagemPorTurmaDto>(json);
            }
            return dto;
        }
        private string MontarUrl(ConsultaSondagemPorTurmaQuery request)
        {
            var queryParams = new List<string>
                {
                    $"TurmaId={request.TurmaId}",
                    $"ProficienciaId={request.ProficienciaId}",
                    $"ComponenteCurricularId={request.ComponenteCurricularId}",
                    $"ModalidadeId={request.ModalidadeId}",
                    $"Ano={request.Ano}",
                    $"AnoLetivo={request.AnoLetivo}",
                    $"Semestre={request.Semestre}"
                };

            if (!string.IsNullOrEmpty(request.UeCodigo))
                queryParams.Add($"UeCodigo={request.UeCodigo}");

            if (request.BimestreId.HasValue)
                queryParams.Add($"BimestreId={request.BimestreId.Value}");

            return $"relatorio-integracao/sondagem-por-turma?{string.Join("&", queryParams)}";
        }
    }
}
