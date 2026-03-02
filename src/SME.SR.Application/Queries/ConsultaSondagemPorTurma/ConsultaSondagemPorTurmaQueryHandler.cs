using MediatR;
using Newtonsoft.Json;
using SME.SR.Infra;
using SME.SR.Infra.Dtos.SondagemTurmaEscritaEF;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application.Queries.ConsultaSondagemPorTurma
{
    public class ConsultaSondagemPorTurmaQueryHandler : IRequestHandler<ConsultaSondagemPorTurmaQuery, ConsultaSondagemPorTurmaDto>
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly VariaveisAmbiente variaveisAmbiente;

        public ConsultaSondagemPorTurmaQueryHandler(IHttpClientFactory httpClientFactory, VariaveisAmbiente variaveisAmbiente)
        {
            this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<ConsultaSondagemPorTurmaDto> Handle(ConsultaSondagemPorTurmaQuery request, CancellationToken cancellationToken)
        {
            var dto = new ConsultaSondagemPorTurmaDto();

            using var httpClient = new HttpClient();
            var url = MontarUrl(request);

            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add("x-api-sondagem-key", variaveisAmbiente.ChaveIntegracaoApiSondagem);
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
                    $"turmaId={request.TurmaId}",
                    $"proficienciaId={request.ProficienciaId}",
                    $"componenteCurricularId={request.ComponenteCurricularId}",
                    $"modalidadeId={request.ModalidadeId}",
                    $"ano={request.Ano}",
                    $"anoLetivo={request.AnoLetivo}",
                    $"semestre={request.Semestre}"
                };

            if (!string.IsNullOrEmpty(request.UeCodigo))
                queryParams.Add($"UeCodigo={request.UeCodigo}");

            if (request.BimestreId.HasValue)
                queryParams.Add($"BimestreId={request.BimestreId.Value}");

            return $"{variaveisAmbiente.UrlApiNovaSondagem}/relatorio-integracao/sondagem-por-turma?{string.Join("&", queryParams)}";
        }
    }
}
