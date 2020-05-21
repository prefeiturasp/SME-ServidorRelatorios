using Microsoft.AspNetCore.Http;
using SME.SR.Infra.Dtos;
using System.Collections.Generic;
using System.IO;
using Refit;
using SME.SR.Infra.Dtos.Requisicao;
using SME.SR.Infra.Dtos.Resposta;
using System;
using System.Threading.Tasks;

namespace SME.SR.JRSClient.Grupos
{
    public interface IReports
    {
        [Headers("Accept: application/json", "Authorization: Basic")]
        [Get("/jasperserver/rest_v2/reportExecutions/{reportExecutionId}/status")]
        Task<string> GetStatusAsync(int reportExecutionId);

        #region Input Controls
        [Headers("Accept: application/json", "Authorization: Basic")]
        [Get("/jasperserver/rest_v2/reports/{**caminhoRelatorio}/inputControls")]
        Task<ListaControlesEntradaDto> GetObterControlesEntradaAsync(string caminhoRelatorio, [AliasAs("exclude")] string ignorarEstados);

        [Headers("Accept: application/json", "Authorization: Basic")]
        [Put("/jasperserver/rest_v2/reports/{**caminhoRelatorio}/inputControls")]
        Task<ListaControlesEntradaDto> MudarOrdemControlesEntradaAsync(string caminhoRelatorio, [Body] ListaControlesEntradaDto listaControlesEntradaDto);

        [Headers("Accept: application/json", "Authorization: Basic")]
        [Post("/jasperserver/rest_v2/reports/{**caminhoRelatorio}/inputControls")]
        Task<ListaControlesEntradaDto> SetarValoresControleEntradaAsync(string caminhoRelatorio, [Body] IDictionary<string, object[]> valoresControles, [Query, AliasAs("freshData")]bool atualizarCache);

        [Headers("Accept: application/json", "Authorization: Basic")]
        [Get("/jasperserver/rest_v2/reports/{**caminhoRelatorio}/inputControls/values")]
        Task<ListaEstadosControleEntradaDto> GetObterEstadosControlesEntradaAsync(string caminhoRelatorio, [AliasAs("freshData")] bool ignorarCache);
        #endregion

        [Headers("Accept: application/json", "Authorization: Basic")]
        [Get("/jasperserver/rest_v2/reports/{**caminhoCompleto}")]
        Task<Stream> GetRelatorioSincrono(string caminhoCompleto, ExecutarRelatorioSincronoDto Dto);

        [Headers("Accept: application/json", "Authorization: Basic")]
        [Get("/jasperserver/rest_v2/jobs")]
        Task<TrabalhoListaResumoDto> GetTrabalhosRelatoriosAsync([AliasAs("reportUnitURI")] string caminhoRelatorio);

        [Headers("Accept: application/json", "Authorization: Basic")]
        [Post("/jasperserver/rest_v2/jobs/pause")]
        Task<TrabalhoListaIdsDto> PostPausarTrabalhosAsync(TrabalhoListaIdsDto trabalhoListaIdsDto);

        [Headers("Accept: application/json", "Authorization: Basic")]
        [Post("/jasperserver/rest_v2/jobs/resume")]
        Task<TrabalhoListaIdsDto> PostInciarTrabalhosAsync(TrabalhoListaIdsDto trabalhoListaIdsDto);

        [Headers("Accept: application/json", "Authorization: Basic")]
        [Post("/jasperserver/rest_v2/jobs/restart")]
        Task<TrabalhoListaIdsDto> PostReinicarTrabalhosFalhadosAsync(TrabalhoListaIdsDto trabalhoListaIdsDto);

        [Headers("Accept: application/json", "Authorization: Basic")]
        [Delete("/jasperserver/rest_v2/jobs/{trabalhoId}")]
        Task<int> DeleteTrabalhoPorIdAsync(int trabalhoId);

        [Headers("Accept: application/json", "Authorization: Basic")]
        [Delete("/jasperserver/rest_v2/jobs")]
        Task<TrabalhoListaIdsDto> DeleteTrabalhosPorListaIdAsync([AliasAs("id"), Query(CollectionFormat.Multi)] IEnumerable<int> ids);

        [Headers("Accept: application/json", "Authorization: Basic")]
        [Get("/jasperserver/rest_v2/jobs")]
        Task<TrabalhoListaResumoDto> GetTrabalhosRelatoriosAsync(TrabalhoFiltroDto filtro, [AliasAs("example")] string? exemplo);

        [Headers("Accept: application/json", "Authorization: Basic")]
        [Get("/jasperserver/rest_v2/jobs/{trabalhoId}")]
        Task<TrabalhoDefinicaoDto> GetTrabalhoRelatorioPorIdAsync(int trabalhoId);

        [Headers("Accept: application/json", "Authorization: Basic", "Content-Type: application/json")]
        [Post("/jasperserver/rest_v2/jobs/{trabalhoId}")]
        Task<TrabalhoDefinicaoDto> PostAlterarDefinicaoTrabalhoAsync(int trabalhoId, TrabalhoDefinicaoDto trabalhoDefinicaoDto);

        [Headers("Accept: application/json", "Authorization: Basic", "Content-Type: application/json")]
        [Post("/jasperserver/rest_v2/jobs")]
        Task<TrabalhoListaIdsDto> PostAtualizarTrabalhosEmLoteAsync([AliasAs("id"),Query(CollectionFormat.Multi)] IEnumerable<int>? ids, bool replaceTriggerIgnoreType, [Body]string? trabalhoDefinicaoJson);

        [Headers("Accept: application/json", "Authorization: Basic")]
        [Get("/jasperserver/rest_v2/jobs/{trabalhoId}/state")]
        Task<TrabalhoEstadoDto> GetTrabalhoEstadoAsync(int trabalhoId);

        [Headers("Accept: application/json", "Authorization: Basic", "Content-Type: application/json")]
        [Put("/jasperserver/rest_v2/jobs")]
        Task<TrabalhoDefinicaoDto> PutAgendarTrabalhoRelatorioAsync(TrabalhoDefinicaoDto definicoesJson);

        [Headers("Accept: application/json", "Authorization: Basic")]
        [Get("/jasperserver/rest_v2/resources")]
        Task<BuscaRepositorioRespostaDto> GetRecursos([Query] BuscaRepositorioRequisicaoDto requisicaoDto);

        [Headers("Accept: application/json", "Authorization: Basic")]
        [Get("/jasperserver/rest_v2/resources{**caminhoRelatorio}")]
        Task<DetalhesRecursoDto> GetDetalhesRecurso(string caminhoRelatorio, [Query] bool expanded);

        [Headers("Accept: application/json", "Authorization: Basic")]
        [Post("/jasperserver/rest_v2/resources{**caminhoRelatorio}")]
        Task<DetalhesRecursoDto> PostRecurso(string caminhoRelatorio, [Header("Content-Location")] string contentLocation, [Query] bool createFolders, [Query] bool? overwrite);

        [Headers("Accept: application/json", "Content-Type: application/repository.folder+json",  "Authorization: Basic")]
        [Post("/jasperserver/rest_v2/resources{**caminhoRelatorio}")]
        Task<DetalhesRecursoDto> PostRecurso(string caminhoRelatorio, [Query] bool createFolders, [Query] bool? overwrite, [Body] DetalhesRecursoDto dto);

        [Headers("Accept: application/json", "Content-Type: application/repository.jndiJdbcDataSource+json", "Authorization: Basic")]
        [Post("/jasperserver/rest_v2/resources{**caminhoRelatorio}")]
        Task<JNDIRecursoDto> PostRecurso(string caminhoRelatorio, [Query] bool createFolders, [Query] bool? overwrite, [Body] JNDIRecursoDto dto);

        [Headers("Accept: application/json", "Content-Type: application/repository.jdbcDataSource+json", "Authorization: Basic")]
        [Post("/jasperserver/rest_v2/resources{**caminhoRelatorio}")]
        Task<JDBCRecursoDto> PostRecurso(string caminhoRelatorio, [Query] bool createFolders, [Query] bool? overwrite, [Body] JDBCRecursoDto dto);

        [Headers("Accept: application/json", "Content-Type: application/repository.awsDataSource+json", "Authorization: Basic")]
        [Post("/jasperserver/rest_v2/resources{**caminhoRelatorio}")]
        Task<AWSRecursoDto> PostRecurso(string caminhoRelatorio, [Query] bool createFolders, [Query] bool? overwrite, [Body] AWSRecursoDto dto);

        [Headers("Accept: application/json", "Content-Type: application/repository.virtualDataSource+json", "Authorization: Basic")]
        [Post("/jasperserver/rest_v2/resources{**caminhoRelatorio}")]
        Task<VirtualRecursoDto> PostRecurso(string caminhoRelatorio, [Query] bool createFolders, [Query] bool? overwrite, [Body] VirtualRecursoDto dto);

        [Headers("Accept: application/json", "Content-Type: application/repository.customDataSource+json", "Authorization: Basic")]
        [Post("/jasperserver/rest_v2/resources{**caminhoRelatorio}")]
        Task<CustomizadoRecursoDto> PostRecurso(string caminhoRelatorio, [Query] bool createFolders, [Query] bool? overwrite, [Body] CustomizadoRecursoDto dto);

        [Headers("Accept: application/json", "Content-Type: application/repository.beanDataSource+json", "Authorization: Basic")]
        [Post("/jasperserver/rest_v2/resources{**caminhoRelatorio}")]
        Task<BeanRecursoDto> PostRecurso(string caminhoRelatorio, [Query] bool createFolders, [Query] bool? overwrite, [Body] BeanRecursoDto dto);

        [Headers("Accept: application/json", "Content-Type: application/repository.dataType+json", "Authorization: Basic")]
        [Post("/jasperserver/rest_v2/resources{**caminhoRelatorio}")]
        Task<TipoInformacaoRecursoDto> PostRecurso(string caminhoRelatorio, [Query] bool createFolders, [Query] bool? overwrite, [Body] TipoInformacaoRecursoDto dto);

        [Headers("Accept: application/json", "Content-Type: application/repository.listOfValues+json", "Authorization: Basic")]
        [Post("/jasperserver/rest_v2/resources{**caminhoRelatorio}")]
        Task<ListaValoresRecursoDto> PostRecurso(string caminhoRelatorio, [Query] bool createFolders, [Query] bool? overwrite, [Body] ListaValoresRecursoDto dto);

        [Headers("Accept: application/json", "Content-Type: application/repository.query+json", "Authorization: Basic")]
        [Post("/jasperserver/rest_v2/resources{**caminhoRelatorio}")]
        Task<QueryRecursoDto> PostRecurso(string caminhoRelatorio, [Query] bool createFolders, [Query] bool? overwrite, [Body] QueryRecursoDto dto);

        [Headers("Accept: application/json", "Content-Type: application/repository.inputControl+json", "Authorization: Basic")]
        [Post("/jasperserver/rest_v2/resources{**caminhoRelatorio}")]
        Task<ControleEntradaDto> PostRecurso(string caminhoRelatorio, [Query] bool createFolders, [Query] bool? overwrite, [Body] ControleEntradaDto dto);

        [Headers("Accept: application/json", "Content-Type: application/repository.file+json", "Authorization: Basic")]
        [Post("/jasperserver/rest_v2/resources{**caminhoRelatorio}")]
        Task<ArquivoRecursoDto> PostRecurso(string caminhoRelatorio, [Query] bool createFolders, [Query] bool? overwrite, [Body] ArquivoRecursoDto dto);

        [Headers("Accept: application/json", "Content-Type: application/repository.reportUnit+json", "Authorization: Basic")]
        [Post("/jasperserver/rest_v2/resources{**caminhoRelatorio}")]
        Task<UnidadeRelatorioRecursoDto> PostRecurso(string caminhoRelatorio, [Query] bool createFolders, [Query] bool? overwrite, [Body] UnidadeRelatorioRecursoDto dto);

        [Headers("Accept: application/json", "Content-Type: application/repository.reportOptions+json", "Authorization: Basic")]
        [Post("/jasperserver/rest_v2/resources{**caminhoRelatorio}")]
        Task<OpcoesRelatorioRecursoDto> PostRecurso(string caminhoRelatorio, [Query] bool createFolders, [Query] bool? overwrite, [Body] OpcoesRelatorioRecursoDto dto);

        [Headers("Accept: application/json", "Content-Type: application/repository.semanticLayerDataSource+json", "Authorization: Basic")]
        [Post("/jasperserver/rest_v2/resources{**caminhoRelatorio}")]
        Task<DominioRecursoDto> PostRecurso(string caminhoRelatorio, [Query] bool createFolders, [Query] bool? overwrite, [Body] DominioRecursoDto dto);

        [Headers("Accept: application/json", "Content-Type: application/repository.domainTopic+json", "Authorization: Basic")]
        [Post("/jasperserver/rest_v2/resources{**caminhoRelatorio}")]
        Task<UnidadeRelatorioRecursoDto> PostRecursoTopicoDominio(string caminhoRelatorio, [Query] bool createFolders, [Query] bool? overwrite, [Body] UnidadeRelatorioRecursoDto dto);

        [Headers("Accept: application/json", "Content-Type: application/repository.xmlaConnection+json", "Authorization: Basic")]
        [Post("/jasperserver/rest_v2/resources{**caminhoRelatorio}")]
        Task<ConexaoXMLARecursoDto> PostRecurso(string caminhoRelatorio, [Query] bool createFolders, [Query] bool? overwrite, [Body] ConexaoXMLARecursoDto dto);

        [Headers("Accept: application/json", "Content-Type: application/repository.mondrianConnection+json", "Authorization: Basic")]
        [Post("/jasperserver/rest_v2/resources{**caminhoRelatorio}")]
        Task<ConexaoMondrianRecursoDto> PostRecurso(string caminhoRelatorio, [Query] bool createFolders, [Query] bool? overwrite, [Body] ConexaoMondrianRecursoDto dto);

        [Headers("Accept: application/json", "Content-Type: application/repository.secureMondrianConnection+json", "Authorization: Basic")]
        [Post("/jasperserver/rest_v2/resources{**caminhoRelatorio}")]
        Task<ConexaoSeguraMondrianRecursoDto> PostRecurso(string caminhoRelatorio, [Query] bool createFolders, [Query] bool? overwrite, [Body] ConexaoSeguraMondrianRecursoDto dto);

        [Headers("Accept: application/json", "Content-Type: application/repository.olapUnit+json", "Authorization: Basic")]
        [Post("/jasperserver/rest_v2/resources{**caminhoRelatorio}")]
        Task<ConexaoOlapRecursoDto> PostRecurso(string caminhoRelatorio, [Query] bool createFolders, [Query] bool? overwrite, [Body] ConexaoOlapRecursoDto dto);

        [Headers("Accept: application/json", "Content-Type: application/repository.mondrianXmlaDefinition+json", "Authorization: Basic")]
        [Post("/jasperserver/rest_v2/resources{**caminhoRelatorio}")]
        Task<MondrianXMLARecursoDto> PostRecurso(string caminhoRelatorio, [Query] bool createFolders, [Query] bool? overwrite, [Body] MondrianXMLARecursoDto dto);

        [Headers("Accept: application/json", "Authorization: Basic")]
        [Put("/jasperserver/rest_v2/resources{**caminhoRelatorio}")]
        Task<DetalhesRecursoDto> PutRecurso(string caminhoRelatorio, [Header("Content-Location")] string contentLocation, [Query] bool createFolders, [Query] bool? overwrite);

        [Headers("Accept: application/json", "Content-Type: application/repository.folder+json", "Authorization: Basic")]
        [Put("/jasperserver/rest_v2/resources{**caminhoRelatorio}")]
        Task<DetalhesRecursoDto> PutRecurso(string caminhoRelatorio, [Query] bool createFolders, [Query] bool? overwrite, [Body] DetalhesRecursoDto dto);

        [Headers("Accept: application/json", "Content-Type: application/repository.jndiJdbcDataSource+json", "Authorization: Basic")]
        [Put("/jasperserver/rest_v2/resources{**caminhoRelatorio}")]
        Task<JNDIRecursoDto> PutRecurso(string caminhoRelatorio, [Query] bool createFolders, [Query] bool? overwrite, [Body] JNDIRecursoDto dto);

        [Headers("Accept: application/json", "Content-Type: application/repository.jdbcDataSource+json", "Authorization: Basic")]
        [Put("/jasperserver/rest_v2/resources{**caminhoRelatorio}")]
        Task<JDBCRecursoDto> PutRecurso(string caminhoRelatorio, [Query] bool createFolders, [Query] bool? overwrite, [Body] JDBCRecursoDto dto);

        [Headers("Accept: application/json", "Content-Type: application/repository.awsDataSource+json", "Authorization: Basic")]
        [Put("/jasperserver/rest_v2/resources{**caminhoRelatorio}")]
        Task<AWSRecursoDto> PutRecurso(string caminhoRelatorio, [Query] bool createFolders, [Query] bool? overwrite, [Body] AWSRecursoDto dto);

        [Headers("Accept: application/json", "Content-Type: application/repository.virtualDataSource+json", "Authorization: Basic")]
        [Put("/jasperserver/rest_v2/resources{**caminhoRelatorio}")]
        Task<VirtualRecursoDto> PutRecurso(string caminhoRelatorio, [Query] bool createFolders, [Query] bool? overwrite, [Body] VirtualRecursoDto dto);

        [Headers("Accept: application/json", "Content-Type: application/repository.customDataSource+json", "Authorization: Basic")]
        [Put("/jasperserver/rest_v2/resources{**caminhoRelatorio}")]
        Task<CustomizadoRecursoDto> PutRecurso(string caminhoRelatorio, [Query] bool createFolders, [Query] bool? overwrite, [Body] CustomizadoRecursoDto dto);

        [Headers("Accept: application/json", "Content-Type: application/repository.beanDataSource+json", "Authorization: Basic")]
        [Put("/jasperserver/rest_v2/resources{**caminhoRelatorio}")]
        Task<BeanRecursoDto> PutRecurso(string caminhoRelatorio, [Query] bool createFolders, [Query] bool? overwrite, [Body] BeanRecursoDto dto);

        [Headers("Accept: application/json", "Content-Type: application/repository.dataType+json", "Authorization: Basic")]
        [Put("/jasperserver/rest_v2/resources{**caminhoRelatorio}")]
        Task<TipoInformacaoRecursoDto> PutRecurso(string caminhoRelatorio, [Query] bool createFolders, [Query] bool? overwrite, [Body] TipoInformacaoRecursoDto dto);

        [Headers("Accept: application/json", "Content-Type: application/repository.listOfValues+json", "Authorization: Basic")]
        [Put("/jasperserver/rest_v2/resources{**caminhoRelatorio}")]
        Task<ListaValoresRecursoDto> PutRecurso(string caminhoRelatorio, [Query] bool createFolders, [Query] bool? overwrite, [Body] ListaValoresRecursoDto dto);

        [Headers("Accept: application/json", "Content-Type: application/repository.query+json", "Authorization: Basic")]
        [Put("/jasperserver/rest_v2/resources{**caminhoRelatorio}")]
        Task<QueryRecursoDto> PutRecurso(string caminhoRelatorio, [Query] bool createFolders, [Query] bool? overwrite, [Body] QueryRecursoDto dto);

        [Headers("Accept: application/json", "Content-Type: application/repository.inputControl+json", "Authorization: Basic")]
        [Put("/jasperserver/rest_v2/resources{**caminhoRelatorio}")]
        Task<ControleEntradaDto> PutRecurso(string caminhoRelatorio, [Query] bool createFolders, [Query] bool? overwrite, [Body] ControleEntradaDto dto);

        [Headers("Accept: application/json", "Content-Type: application/repository.file+json", "Authorization: Basic")]
        [Put("/jasperserver/rest_v2/resources{**caminhoRelatorio}")]
        Task<ArquivoRecursoDto> PutRecurso(string caminhoRelatorio, [Query] bool createFolders, [Query] bool? overwrite, [Body] ArquivoRecursoDto dto);

        [Headers("Accept: application/json", "Content-Type: application/repository.reportUnit+json", "Authorization: Basic")]
        [Put("/jasperserver/rest_v2/resources{**caminhoRelatorio}")]
        Task<UnidadeRelatorioRecursoDto> PutRecurso(string caminhoRelatorio, [Query] bool createFolders, [Query] bool? overwrite, [Body] UnidadeRelatorioRecursoDto dto);

        [Headers("Accept: application/json", "Content-Type: application/repository.reportOptions+json", "Authorization: Basic")]
        [Put("/jasperserver/rest_v2/resources{**caminhoRelatorio}")]
        Task<OpcoesRelatorioRecursoDto> PutRecurso(string caminhoRelatorio, [Query] bool createFolders, [Query] bool? overwrite, [Body] OpcoesRelatorioRecursoDto dto);

        [Headers("Accept: application/json", "Content-Type: application/repository.semanticLayerDataSource+json", "Authorization: Basic")]
        [Put("/jasperserver/rest_v2/resources{**caminhoRelatorio}")]
        Task<DominioRecursoDto> PutRecurso(string caminhoRelatorio, [Query] bool createFolders, [Query] bool? overwrite, [Body] DominioRecursoDto dto);

        [Headers("Accept: application/json", "Content-Type: application/repository.domainTopic+json", "Authorization: Basic")]
        [Put("/jasperserver/rest_v2/resources{**caminhoRelatorio}")]
        Task<UnidadeRelatorioRecursoDto> PutRecursoTopicoDominio(string caminhoRelatorio, [Query] bool createFolders, [Query] bool? overwrite, [Body] UnidadeRelatorioRecursoDto dto);

        [Headers("Accept: application/json", "Content-Type: application/repository.xmlaConnection+json", "Authorization: Basic")]
        [Put("/jasperserver/rest_v2/resources{**caminhoRelatorio}")]
        Task<ConexaoXMLARecursoDto> PutRecurso(string caminhoRelatorio, [Query] bool createFolders, [Query] bool? overwrite, [Body] ConexaoXMLARecursoDto dto);

        [Headers("Accept: application/json", "Content-Type: application/repository.mondrianConnection+json", "Authorization: Basic")]
        [Put("/jasperserver/rest_v2/resources{**caminhoRelatorio}")]
        Task<ConexaoMondrianRecursoDto> PutRecurso(string caminhoRelatorio, [Query] bool createFolders, [Query] bool? overwrite, [Body] ConexaoMondrianRecursoDto dto);

        [Headers("Accept: application/json", "Content-Type: application/repository.secureMondrianConnection+json", "Authorization: Basic")]
        [Put("/jasperserver/rest_v2/resources{**caminhoRelatorio}")]
        Task<ConexaoSeguraMondrianRecursoDto> PutRecurso(string caminhoRelatorio, [Query] bool createFolders, [Query] bool? overwrite, [Body] ConexaoSeguraMondrianRecursoDto dto);

        [Headers("Accept: application/json", "Content-Type: application/repository.olapUnit+json", "Authorization: Basic")]
        [Put("/jasperserver/rest_v2/resources{**caminhoRelatorio}")]
        Task<ConexaoOlapRecursoDto> PutRecurso(string caminhoRelatorio, [Query] bool createFolders, [Query] bool? overwrite, [Body] ConexaoOlapRecursoDto dto);

        [Headers("Accept: application/json", "Content-Type: application/repository.mondrianXmlaDefinition+json", "Authorization: Basic")]
        [Put("/jasperserver/rest_v2/resources{**caminhoRelatorio}")]
        Task<MondrianXMLARecursoDto> PutRecurso(string caminhoRelatorio, [Query] bool createFolders, [Query] bool? overwrite, [Body] MondrianXMLARecursoDto dto);

        [Headers("Accept: application/json", "Authorization: Basic")]
        [Delete("/jasperserver/rest_v2/resources{**caminhoRelatorio}")]
        Task<DetalhesRecursoDto> DeleteRecurso(string caminhoRelatorio, [Query] string resourceUri);

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
