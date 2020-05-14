using Refit;
using SME.SR.Infra.Dtos;
using SME.SR.Infra.Dtos.Requisicao.ExecucaoRelatorio.GetRelatoriosTarefasEmAndamento;
using SME.SR.Infra.Dtos.Requisicao.ExecucaoRelatorio.PostExecucaoRelatorioAsync;
using SME.SR.Infra.Dtos.Requisicao.ExecucaoRelatorio.PostExportacaoRelatorioAsync;
using SME.SR.Infra.Dtos.Requisicao.ExecucaoRelatorio.PostModificarParametrosRelatorio;
using SME.SR.Infra.Dtos.Resposta.ExecucaoRelatorio.GetDetalhesExecucaoRelatorio;
using SME.SR.Infra.Dtos.Resposta.ExecucaoRelatorio.GetRelatoriosTarefasEmAndamento;
using SME.SR.Infra.Dtos.Resposta.ExecucaoRelatorio.PostExecucaoRelatorioAsync;
using SME.SR.Infra.Dtos.Resposta.ExecucaoRelatorio.PostExportacaoRelatorioAsync;
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
        [Get("/jasperserver/rest_v2/reportExecutions/{requestID}/status/")]
        Task<string> GetPoolExecucaoRelatorio([Header("Authorization")] string authorization, [AliasAs("requestID")] Guid requestId);

        [Headers("Accept: application/json")]
        [Get("/jasperserver/rest_v2/reportExecutions/{requestID}")]
        Task<DetalhesExecucaoRelatorioRespostaDto> GetDetalhesExecucaoRelatorio([Header("Authorization")] string authorization, [AliasAs("requestID")] Guid requestId);

        [Headers("Accept: application/json")]
        [Get("/jasperserver/rest_v2/reportExecutions/{requestID}/exports/{exportID}/outputResource")]
        Task<string> GetSaidaRelatorio([Header("Authorization")] string authorization, [AliasAs("requestID")] Guid requestId, [AliasAs("exportID")] Guid exportID);

        [Headers("Accept: application/json")]
        [Get("/jasperserver/rest_v2/reportExecutions/{requestID}/exports/{exportID}/attachments/{fileName}")]
        Task<string> GetAnexosRelatorio([Header("Authorization")] string authorization, [AliasAs("requestID")] Guid requestId, [AliasAs("exportID")] Guid exportID, [AliasAs("fileName")] string fileName);

        [Headers("Accept: application/json")]
        [Post("/jasperserver/rest_v2/reportExecutions/{requestID}/exports/")]
        Task<ExportacaoRelatorioRespostaDto> PostExportacaoRelatorioAsync([Header("Authorization")] string authorization, [AliasAs("requestID")] Guid requestId, [Body]ExportacaoRelatorioRequisicaoDto requisicao);

        [Headers("Accept: application/json")]
        [Post("/jasperserver/rest_v2/reportExecutions/{requestID}/parameters/")]
        Task<string> PostModificarParametrosRelatorio([Header("Authorization")] string authorization, [AliasAs("requestID")] Guid requestId, [Body]ModificarParametrosRelatorioRequisicaoDto requisicao );

        [Headers("Accept: application/json")]
        [Get("/jasperserver/rest_v2/reportExecutions/{requestID}/exports/{exportID}/status")]
        Task<string> GetPoolExecucaoExportacao([Header("Authorization")] string authorization, [AliasAs("requestID")] Guid requestId, [AliasAs("exportID")] Guid exportID);

        [Headers("Accept: application/json")]
        [Get("/jasperserver/rest_v2/reportExecutions")]
        Task<RelatoriosTarefasEmAndamentoRespostaDto> GetRelatoriosTarefasEmAndamento([Header("Authorization")] string authorization, [Query]RelatoriosTarefasEmAndamentoRequisicaoDto requisicao);

        [Headers("Accept: application/json")]
        [Put("/jasperserver/rest_v2/reportExecutions/{requestID}/status/")]
        Task<string> PutInterromperRelatoriosTarefas([Header("Authorization")] string authorization, [AliasAs("requestID")] Guid requestId);
    }
}
