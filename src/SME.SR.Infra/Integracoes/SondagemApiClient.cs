using Newtonsoft.Json;
using SME.SR.Infra.Dtos.NovoSondagem;
using SME.SR.Infra.Interfaces.Integracoes;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SME.SR.Infra.Integracoes
{
    public class SondagemApiClient : ISondagemApiClient
    {
        private readonly HttpClient httpClient;
        private readonly VariaveisAmbiente variaveis;

        public SondagemApiClient(
            HttpClient httpClient,
            VariaveisAmbiente variaveis)
        {
            this.httpClient = httpClient;
            this.variaveis = variaveis;
        }

        public async Task<RetornoApiSondagemQuestionarioDto>
            ObterDadosQuestionarioAsync(FiltroRelatorioSondagemQuestionarioDto filtro)
        {
            var url = $"{variaveis.UrlApiNovaSondagem}/relatorio-integracao/sondagem-por-turma" +
                      $"?TurmaId={filtro.TurmaId}" +
                      $"&ProficienciaId={filtro.ProficienciaId}" +
                      (filtro.ComponenteCurricularId > 0 ? $"&ComponenteCurricularId={filtro.ComponenteCurricularId}" : string.Empty) +
                      $"&Modalidade={filtro.Modalidade}" +
                      $"&Ano={filtro.Ano}" +
                      $"&AnoLetivo={filtro.AnoLetivo}" +
                      (filtro.SemestreId > 0 ? $"&SemestreId={filtro.SemestreId}" : string.Empty) +
                      (!string.IsNullOrWhiteSpace(filtro.UeCodigo) ? $"&UeCodigo={filtro.UeCodigo}" : string.Empty) +
                      (filtro.BimestreId.HasValue && filtro.BimestreId > 0 ? $"&BimestreId={filtro.BimestreId}" : string.Empty);

            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add(
                "x-api-sondagem-key",
                variaveis.ChaveIntegracaoApiSondagem);

            var response = await httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Erro ao consultar API de sondagem. Status: {response.StatusCode}");

            var json = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<RetornoApiSondagemQuestionarioDto>(json)
                   ?? throw new Exception("Retorno da API veio nulo.");
        }
    }
}