using Newtonsoft.Json;
using SME.SR.Infra.Interfaces.Integracoes;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Infra.Integracoes
{
    public class SolicitacaoRelatorioApiClient : ISolicitacaoRelatorioApiClient
    {
        private readonly HttpClient httpClient;
        private readonly VariaveisAmbiente variaveis;

        public SolicitacaoRelatorioApiClient(
            HttpClient httpClient,
            VariaveisAmbiente variaveis)
        {
            this.httpClient = httpClient;
            this.variaveis = variaveis;
        }

        public async Task FinalizarSolicitacaoAsync(int solicitacaoRelatorioId)
        {
            var url = $"{variaveis.UrlApiNovoSgp}v1/solicitacao-relatorio/finalizar-solicitacao";

            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add(
                "x-sgp-api-key",
                variaveis.ChaveIntegracaoApiSgp);

            var payload = JsonConvert.SerializeObject(solicitacaoRelatorioId);
            var content = new StringContent(payload, Encoding.UTF8, "application/json");

            try
            {
                var response = await httpClient.PatchAsync(url, content);

                if (!response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(
                        $"[AVISO] Erro ao finalizar solicitação {solicitacaoRelatorioId}. " +
                        $"Status: {response.StatusCode}. Body: {responseBody}");
                    return;
                }

                Console.WriteLine($">>> Solicitação {solicitacaoRelatorioId} finalizada com sucesso.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"[AVISO] Exceção ao finalizar solicitação {solicitacaoRelatorioId}: {ex.Message}");
            }
        }
    }
}