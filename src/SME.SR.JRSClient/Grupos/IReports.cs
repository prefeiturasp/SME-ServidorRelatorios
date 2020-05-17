using Refit;
using SME.SR.Infra.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.JRSClient.Grupos
{
    public interface IReports
    {
        [Headers("Accept: application/json", "Authorization: Basic")]
        [Get("/jasperserver/rest_v2/reportExecutions/123/status/")]
        Task<string> GetStatusAsync();

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
        [Get("/jasperserver/rest_v2/resources")]
        Task<BuscaRepositorioRespostaDto> GetRecursos([Query] BuscaRepositorioRequisicaoDto requisicaoDto);

        [Headers("Accept: application/json", "Authorization: Basic")]
        [Get("/jasperserver/rest_v2/resources{**caminhoRelatorio}")]
        Task<DetalhesRecursoDto> GetDetalhesRecurso(string caminhoRelatorio, [Query] bool expanded);

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

    }
}
