using Refit;
using SME.SR.Infra.Dtos;
using System.Threading.Tasks;

namespace SME.SR.JRSClient.Grupos
{
    public interface IReports
    {
        [Headers("Accept: application/json")]
        [Post("/jasperserver/rest_v2/reportExecutions")]
        Task<ExecucaoRelatorioRespostaDto> PostExecucoesRelatoriosAsync(ExecucaoRelatorioRequisicaoDto request);
    }
}
