using Refit;
using SME.SR.Infra.Dtos.Requisicao;
using SME.SR.Infra.Dtos.Resposta;
using System;
using System.Threading.Tasks;

namespace SME.SR.JRSClient.Grupos
{
    public interface IReports
    {
        [Headers("Accept: application/json")]
        [Post("/jasperserver/rest_v2/reportExecutions")]
        Task<ExecucaoRelatorioRespostaDto> PostExecucaoRelatorioAsync([Header("Authorization")] string authorization, [Body]ExecucaoRelatorioRequisicaoDto request);

        [Headers("Accept: application/json")]
        [Get("/jasperserver/rest_v2/reportExecutions/{requestId}/status/")]
        Task<ApiResponse<string>> GetPoolExecucaoRelatorio([Header("Authorization")] string authorization, Guid requestId);

        [Headers("Accept: application/json")]
        [Get("/jasperserver/rest_v2/reportExecutions/{requestID}")]
        Task<ApiResponse<DetalhesExecucaoRelatorioRespostaDto>> GetDetalhesExecucaoRelatorio([Header("Authorization")] string authorization, [AliasAs("requestID")] Guid requestId);

        [Headers("Accept: application/json")]
        [Get("/jasperserver/rest_v2/reportExecutions/{requestID}/exports/{exportID}/outputResource")]
        Task<ApiResponse<string>> GetSaidaRelatorio([Header("Authorization")] string authorization, [AliasAs("requestID")] Guid requestId, [AliasAs("exportID")] Guid exportID);

        [Headers("Accept: application/json")]
        [Get("/jasperserver/rest_v2/reportExecutions/{requestID}/exports/{exportID}/attachments/{fileName}")]
        Task<ApiResponse<string>> GetAnexosRelatorio([Header("Authorization")] string authorization, [AliasAs("requestID")] Guid requestId, [AliasAs("exportID")] Guid exportID, [AliasAs("fileName")] string fileName);

        [Headers("Accept: application/json")]
        [Post("/jasperserver/rest_v2/reportExecutions/{requestID}/exports/")]
        Task<ApiResponse<ExportacaoRelatorioRespostaDto>> PostExportacaoRelatorioAsync([Header("Authorization")] string authorization, [AliasAs("requestID")] Guid requestId, [Body]ExportacaoRelatorioRequisicaoDto requisicao);

        [Headers("Accept: application/json")]
        [Post("/jasperserver/rest_v2/reportExecutions/{requestID}/parameters/")]
        Task<ApiResponse<string>> PostModificarParametrosRelatorio([Header("Authorization")] string authorization, [AliasAs("requestID")] Guid requestId, [Body]ModificarParametrosRelatorioRequisicaoDto requisicao );

        [Headers("Accept: application/json")]
        [Get("/jasperserver/rest_v2/reportExecutions/{requestID}/exports/{exportID}/status")]
        Task<ApiResponse<string>> GetPoolExecucaoExportacao([Header("Authorization")] string authorization, [AliasAs("requestID")] Guid requestId, [AliasAs("exportID")] Guid exportID);

        [Headers("Accept: application/json")]
        [Get("/jasperserver/rest_v2/reportExecutions")]
        Task<ApiResponse<RelatoriosTarefasEmAndamentoRespostaDto>> GetRelatoriosTarefasEmAndamento([Header("Authorization")] string authorization, [Query]RelatoriosTarefasEmAndamentoRequisicaoDto requisicao);

        [Headers("Accept: application/json")]
        [Put("/jasperserver/rest_v2/reportExecutions/{requestID}/status/")]
        Task<ApiResponse<string>> PutInterromperRelatoriosTarefas([Header("Authorization")] string authorization, [AliasAs("requestID")] Guid requestId);
    }
}
