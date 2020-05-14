using Refit;
using SME.SR.Infra.Dtos;
using System.Threading.Tasks;

namespace SME.SR.JRSClient.Grupos
{
    public interface IReports
    {
        [Headers("Accept: application/json")]

        [Get("/jasperserver/rest_v2/reportExecutions/123/status/")]
        Task<string> GetStatusAsync([Header("Authorization")] string authorization);
        [Headers("Accept: application/json")]
        [Post("/jasperserver/rest_v2/reportExecutions")]
        Task<ExecucaoRelatorioRespostaDto> PostExecucoesRelatoriosAsync(ExecucaoRelatorioRequisicaoDto request);
    }
}
