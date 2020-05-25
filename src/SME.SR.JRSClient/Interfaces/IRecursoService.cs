using SME.SR.Infra.Dtos;
using System.Threading.Tasks;

namespace SME.SR.JRSClient.Interfaces
{
    public interface IRecursoService
    {
        Task<BuscaRepositorioRespostaDto> BuscarRepositorio(BuscaRepositorioRequisicaoDto requisicaoDto);

        Task<DetalhesRecursoDto> ObterDetalhesRecurso(string caminhoRelatorio, bool expandido);

        Task<DetalhesRecursoDto> Post(string caminhoRelatorio, string caminhoConteudo, bool criarDiretorio, bool? sobrescrever);

        Task<DetalhesRecursoDto> Post(string caminhoRelatorio, bool criarDiretorio, bool? sobrescrever, DetalhesRecursoDto dto);

        Task<JNDIRecursoDto> Post(string caminhoRelatorio, bool criarDiretorio, bool? sobrescrever, JNDIRecursoDto dto);

        Task<JDBCRecursoDto> Post(string caminhoRelatorio, bool criarDiretorio, bool? sobrescrever, JDBCRecursoDto dto);

        Task<AWSRecursoDto> Post(string caminhoRelatorio, bool criarDiretorio, bool? sobrescrever, AWSRecursoDto dto);

        Task<VirtualRecursoDto> Post(string caminhoRelatorio, bool criarDiretorio, bool? sobrescrever, VirtualRecursoDto dto);

        Task<CustomizadoRecursoDto> Post(string caminhoRelatorio, bool criarDiretorio, bool? sobrescrever, CustomizadoRecursoDto dto);

        Task<BeanRecursoDto> Post(string caminhoRelatorio, bool criarDiretorio, bool? sobrescrever, BeanRecursoDto dto);

        Task<TipoInformacaoRecursoDto> Post(string caminhoRelatorio, bool criarDiretorio, bool? sobrescrever, TipoInformacaoRecursoDto dto);

        Task<ListaValoresRecursoDto> Post(string caminhoRelatorio, bool criarDiretorio, bool? sobrescrever, ListaValoresRecursoDto dto);

        Task<QueryRecursoDto> Post(string caminhoRelatorio, bool criarDiretorio, bool? sobrescrever, QueryRecursoDto dto);

        Task<ControleEntradaDto> Post(string caminhoRelatorio, bool criarDiretorio, bool? sobrescrever, ControleEntradaDto dto);

        Task<ArquivoRecursoDto> Post(string caminhoRelatorio, bool criarDiretorio, bool? sobrescrever, ArquivoRecursoDto dto);

        Task<UnidadeRelatorioRecursoDto> CriarRelatorio(string caminhoRelatorio, bool criarDiretorio, bool? sobrescrever, UnidadeRelatorioRecursoDto dto);

        Task<OpcoesRelatorioRecursoDto> Post(string caminhoRelatorio, bool criarDiretorio, bool? sobrescrever, OpcoesRelatorioRecursoDto dto);

        Task<DominioRecursoDto> Post(string caminhoRelatorio, bool criarDiretorio, bool? sobrescrever, DominioRecursoDto dto);

        Task<UnidadeRelatorioRecursoDto> PostTopicoDominio(string caminhoRelatorio, bool criarDiretorio, bool? sobrescrever, UnidadeRelatorioRecursoDto dto);

        Task<ConexaoXMLARecursoDto> Post(string caminhoRelatorio, bool criarDiretorio, bool? sobrescrever, ConexaoXMLARecursoDto dto);

        Task<ConexaoMondrianRecursoDto> Post(string caminhoRelatorio, bool criarDiretorio, bool? sobrescrever, ConexaoMondrianRecursoDto dto);

        Task<ConexaoSeguraMondrianRecursoDto> Post(string caminhoRelatorio, bool criarDiretorio, bool? sobrescrever, ConexaoSeguraMondrianRecursoDto dto);

        Task<ConexaoOlapRecursoDto> Post(string caminhoRelatorio, bool criarDiretorio, bool? sobrescrever, ConexaoOlapRecursoDto dto);

        Task<MondrianXMLARecursoDto> Post(string caminhoRelatorio, bool criarDiretorio, bool? sobrescrever, MondrianXMLARecursoDto dto);

        Task<DetalhesRecursoDto> Put(string caminhoRelatorio, string caminhoConteudo, bool criarDiretorio, bool? sobrescrever);

        Task<DetalhesRecursoDto> Put(string caminhoRelatorio, bool criarDiretorio, bool? sobrescrever, DetalhesRecursoDto dto);

        Task<JNDIRecursoDto> Put(string caminhoRelatorio, bool criarDiretorio, bool? sobrescrever, JNDIRecursoDto dto);

        Task<JDBCRecursoDto> Put(string caminhoRelatorio, bool criarDiretorio, bool? sobrescrever, JDBCRecursoDto dto);

        Task<AWSRecursoDto> Put(string caminhoRelatorio, bool criarDiretorio, bool? sobrescrever, AWSRecursoDto dto);

        Task<VirtualRecursoDto> Put(string caminhoRelatorio, bool criarDiretorio, bool? sobrescrever, VirtualRecursoDto dto);

        Task<CustomizadoRecursoDto> Put(string caminhoRelatorio, bool criarDiretorio, bool? sobrescrever, CustomizadoRecursoDto dto);

        Task<BeanRecursoDto> Put(string caminhoRelatorio, bool criarDiretorio, bool? sobrescrever, BeanRecursoDto dto);

        Task<TipoInformacaoRecursoDto> Put(string caminhoRelatorio, bool criarDiretorio, bool? sobrescrever, TipoInformacaoRecursoDto dto);

        Task<ListaValoresRecursoDto> Put(string caminhoRelatorio, bool criarDiretorio, bool? sobrescrever, ListaValoresRecursoDto dto);

        Task<QueryRecursoDto> Put(string caminhoRelatorio, bool criarDiretorio, bool? sobrescrever, QueryRecursoDto dto);

        Task<ControleEntradaDto> Put(string caminhoRelatorio, bool criarDiretorio, bool? sobrescrever, ControleEntradaDto dto);

        Task<ArquivoRecursoDto> Put(string caminhoRelatorio, bool criarDiretorio, bool? sobrescrever, ArquivoRecursoDto dto);

        Task<UnidadeRelatorioRecursoDto> Put(string caminhoRelatorio, bool criarDiretorio, bool? sobrescrever, UnidadeRelatorioRecursoDto dto);

        Task<OpcoesRelatorioRecursoDto> Put(string caminhoRelatorio, bool criarDiretorio, bool? sobrescrever, OpcoesRelatorioRecursoDto dto);

        Task<DominioRecursoDto> Put(string caminhoRelatorio, bool criarDiretorio, bool? sobrescrever, DominioRecursoDto dto);

        Task<UnidadeRelatorioRecursoDto> PutTopicoDominio(string caminhoRelatorio, bool criarDiretorio, bool? sobrescrever, UnidadeRelatorioRecursoDto dto);

        Task<ConexaoXMLARecursoDto> Put(string caminhoRelatorio, bool criarDiretorio, bool? sobrescrever, ConexaoXMLARecursoDto dto);

        Task<ConexaoMondrianRecursoDto> Put(string caminhoRelatorio, bool criarDiretorio, bool? sobrescrever, ConexaoMondrianRecursoDto dto);

        Task<ConexaoSeguraMondrianRecursoDto> Put(string caminhoRelatorio, bool criarDiretorio, bool? sobrescrever, ConexaoSeguraMondrianRecursoDto dto);

        Task<ConexaoOlapRecursoDto> Put(string caminhoRelatorio, bool criarDiretorio, bool? sobrescrever, ConexaoOlapRecursoDto dto);

        Task<MondrianXMLARecursoDto> Put(string caminhoRelatorio, bool criarDiretorio, bool? sobrescrever, MondrianXMLARecursoDto dto);

        Task<DetalhesRecursoDto> Delete(string caminhoRelatorio, string caminhoRecurso);
    }
}
