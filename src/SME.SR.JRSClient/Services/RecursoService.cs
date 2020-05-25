using SME.SR.Infra.Dtos;
using SME.SR.JRSClient.Grupos;
using SME.SR.JRSClient.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;

namespace SME.SR.JRSClient.Services
{
    public class RecursoService : ServiceBase<IReports>, IRecursoService
    {
        public RecursoService(Configuracoes configuracoes) : base(configuracoes)
        {
        }

        public RecursoService(HttpClient httpClient, Configuracoes configuracoes) : base(httpClient, configuracoes)
        {
        }

        public async Task<BuscaRepositorioRespostaDto> BuscarRepositorio(BuscaRepositorioRequisicaoDto requisicaoDto)
        {
            return await restService.GetRecursos(requisicaoDto);
        }

        public async Task<DetalhesRecursoDto> Delete(string caminhoRelatorio, string caminhoRecurso)
        {
            return await restService.DeleteRecurso(caminhoRelatorio, caminhoRecurso);
        }

        public async Task<DetalhesRecursoDto> ObterDetalhesRecurso(string caminhoRelatorio, bool expanded)
        {
            return await restService.GetDetalhesRecurso(caminhoRelatorio, expanded);
        }

        public async Task<DetalhesRecursoDto> Post(string caminhoRelatorio, bool criarDiretorio, bool? sobrescrever, DetalhesRecursoDto dto)
        {
            return await restService.PostRecurso(caminhoRelatorio, criarDiretorio, sobrescrever, dto);
        }

        public async Task<JNDIRecursoDto> Post(string caminhoRelatorio, bool criarDiretorio, bool? sobrescrever, JNDIRecursoDto dto)
        {
            return await restService.PostRecurso(caminhoRelatorio, criarDiretorio, sobrescrever, dto);
        }

        public async Task<JDBCRecursoDto> Post(string caminhoRelatorio, bool criarDiretorio, bool? sobrescrever, JDBCRecursoDto dto)
        {
            return await restService.PostRecurso(caminhoRelatorio, criarDiretorio, sobrescrever, dto);
        }

        public async Task<AWSRecursoDto> Post(string caminhoRelatorio, bool criarDiretorio, bool? sobrescrever, AWSRecursoDto dto)
        {
            return await restService.PostRecurso(caminhoRelatorio, criarDiretorio, sobrescrever, dto);
        }

        public async Task<VirtualRecursoDto> Post(string caminhoRelatorio, bool criarDiretorio, bool? sobrescrever, VirtualRecursoDto dto)
        {
            return await restService.PostRecurso(caminhoRelatorio, criarDiretorio, sobrescrever, dto);
        }

        public async Task<CustomizadoRecursoDto> Post(string caminhoRelatorio, bool criarDiretorio, bool? sobrescrever, CustomizadoRecursoDto dto)
        {
            return await restService.PostRecurso(caminhoRelatorio, criarDiretorio, sobrescrever, dto);
        }

        public async Task<BeanRecursoDto> Post(string caminhoRelatorio, bool criarDiretorio, bool? sobrescrever, BeanRecursoDto dto)
        {
            return await restService.PostRecurso(caminhoRelatorio, criarDiretorio, sobrescrever, dto);
        }

        public async Task<TipoInformacaoRecursoDto> Post(string caminhoRelatorio, bool criarDiretorio, bool? sobrescrever, TipoInformacaoRecursoDto dto)
        {
            return await restService.PostRecurso(caminhoRelatorio, criarDiretorio, sobrescrever, dto);
        }

        public async Task<ListaValoresRecursoDto> Post(string caminhoRelatorio, bool criarDiretorio, bool? sobrescrever, ListaValoresRecursoDto dto)
        {
            return await restService.PostRecurso(caminhoRelatorio, criarDiretorio, sobrescrever, dto);
        }

        public async Task<QueryRecursoDto> Post(string caminhoRelatorio, bool criarDiretorio, bool? sobrescrever, QueryRecursoDto dto)
        {
            return await restService.PostRecurso(caminhoRelatorio, criarDiretorio, sobrescrever, dto);
        }

        public async Task<ControleEntradaDto> Post(string caminhoRelatorio, bool criarDiretorio, bool? sobrescrever, ControleEntradaDto dto)
        {
            return await restService.PostRecurso(caminhoRelatorio, criarDiretorio, sobrescrever, dto);
        }

        public async Task<ArquivoRecursoDto> Post(string caminhoRelatorio, bool criarDiretorio, bool? sobrescrever, ArquivoRecursoDto dto)
        {
            return await restService.PostRecurso(caminhoRelatorio, criarDiretorio, sobrescrever, dto);
        }

        public async Task<UnidadeRelatorioRecursoDto> CriarRelatorio(string caminhoRelatorio, bool criarDiretorio, bool? sobrescrever, UnidadeRelatorioRecursoDto dto)
        {
            return await restService.CriarRecursoRelatorio(caminhoRelatorio, criarDiretorio, sobrescrever, dto);
        }

        public async Task<OpcoesRelatorioRecursoDto> Post(string caminhoRelatorio, bool criarDiretorio, bool? sobrescrever, OpcoesRelatorioRecursoDto dto)
        {
            return await restService.PostRecurso(caminhoRelatorio, criarDiretorio, sobrescrever, dto);
        }

        public async Task<DominioRecursoDto> Post(string caminhoRelatorio, bool criarDiretorio, bool? sobrescrever, DominioRecursoDto dto)
        {
            return await restService.PostRecurso(caminhoRelatorio, criarDiretorio, sobrescrever, dto);
        }

        public async Task<ConexaoXMLARecursoDto> Post(string caminhoRelatorio, bool criarDiretorio, bool? sobrescrever, ConexaoXMLARecursoDto dto)
        {
            return await restService.PostRecurso(caminhoRelatorio, criarDiretorio, sobrescrever, dto);
        }

        public async Task<ConexaoMondrianRecursoDto> Post(string caminhoRelatorio, bool criarDiretorio, bool? sobrescrever, ConexaoMondrianRecursoDto dto)
        {
            return await restService.PostRecurso(caminhoRelatorio, criarDiretorio, sobrescrever, dto);
        }

        public async Task<ConexaoSeguraMondrianRecursoDto> Post(string caminhoRelatorio, bool criarDiretorio, bool? sobrescrever, ConexaoSeguraMondrianRecursoDto dto)
        {
            return await restService.PostRecurso(caminhoRelatorio, criarDiretorio, sobrescrever, dto);
        }

        public async Task<ConexaoOlapRecursoDto> Post(string caminhoRelatorio, bool criarDiretorio, bool? sobrescrever, ConexaoOlapRecursoDto dto)
        {
            return await restService.PostRecurso(caminhoRelatorio, criarDiretorio, sobrescrever, dto);
        }

        public async Task<MondrianXMLARecursoDto> Post(string caminhoRelatorio, bool criarDiretorio, bool? sobrescrever, MondrianXMLARecursoDto dto)
        {
            return await restService.PostRecurso(caminhoRelatorio, criarDiretorio, sobrescrever, dto);
        }

        public async Task<DetalhesRecursoDto> Post(string caminhoRelatorio, string caminhoConteudo, bool criarDiretorio, bool? sobrescrever)
        {
            return await restService.PostRecurso(caminhoRelatorio, caminhoConteudo, criarDiretorio, sobrescrever);
        }

        public async Task<UnidadeRelatorioRecursoDto> PostTopicoDominio(string caminhoRelatorio, bool criarDiretorio, bool? sobrescrever, UnidadeRelatorioRecursoDto dto)
        {
            return await restService.PostRecursoTopicoDominio(caminhoRelatorio, criarDiretorio, sobrescrever, dto);
        }

        public async Task<DetalhesRecursoDto> Put(string caminhoRelatorio, bool criarDiretorio, bool? sobrescrever, DetalhesRecursoDto dto)
        {
            return await restService.PutRecurso(caminhoRelatorio, criarDiretorio, sobrescrever, dto);
        }

        public async Task<JNDIRecursoDto> Put(string caminhoRelatorio, bool criarDiretorio, bool? sobrescrever, JNDIRecursoDto dto)
        {
            return await restService.PutRecurso(caminhoRelatorio, criarDiretorio, sobrescrever, dto);
        }

        public async Task<JDBCRecursoDto> Put(string caminhoRelatorio, bool criarDiretorio, bool? sobrescrever, JDBCRecursoDto dto)
        {
            return await restService.PutRecurso(caminhoRelatorio, criarDiretorio, sobrescrever, dto);
        }

        public async Task<AWSRecursoDto> Put(string caminhoRelatorio, bool criarDiretorio, bool? sobrescrever, AWSRecursoDto dto)
        {
            return await restService.CriarRecursoAWSDatsource(caminhoRelatorio, criarDiretorio, sobrescrever, dto);
        }

        public async Task<VirtualRecursoDto> Put(string caminhoRelatorio, bool criarDiretorio, bool? sobrescrever, VirtualRecursoDto dto)
        {
            return await restService.CriarRecursoVirtualDataSource(caminhoRelatorio, criarDiretorio, sobrescrever, dto);
        }

        public async Task<CustomizadoRecursoDto> Put(string caminhoRelatorio, bool criarDiretorio, bool? sobrescrever, CustomizadoRecursoDto dto)
        {
            return await restService.CriarRecursoCustomDataSource(caminhoRelatorio, criarDiretorio, sobrescrever, dto);
        }

        public async Task<BeanRecursoDto> Put(string caminhoRelatorio, bool criarDiretorio, bool? sobrescrever, BeanRecursoDto dto)
        {
            return await restService.CriarRecursoBeanDataSource(caminhoRelatorio, criarDiretorio, sobrescrever, dto);
        }

        public async Task<TipoInformacaoRecursoDto> Put(string caminhoRelatorio, bool criarDiretorio, bool? sobrescrever, TipoInformacaoRecursoDto dto)
        {
            return await restService.PutRecurso(caminhoRelatorio, criarDiretorio, sobrescrever, dto);
        }

        public async Task<ListaValoresRecursoDto> Put(string caminhoRelatorio, bool criarDiretorio, bool? sobrescrever, ListaValoresRecursoDto dto)
        {
            return await restService.PutRecurso(caminhoRelatorio, criarDiretorio, sobrescrever, dto);
        }

        public async Task<QueryRecursoDto> Put(string caminhoRelatorio, bool criarDiretorio, bool? sobrescrever, QueryRecursoDto dto)
        {
            return await restService.PutRecurso(caminhoRelatorio, criarDiretorio, sobrescrever, dto);
        }

        public async Task<ControleEntradaDto> Put(string caminhoRelatorio, bool criarDiretorio, bool? sobrescrever, ControleEntradaDto dto)
        {
            return await restService.PutRecurso(caminhoRelatorio, criarDiretorio, sobrescrever, dto);
        }

        public async Task<ArquivoRecursoDto> Put(string caminhoRelatorio, bool criarDiretorio, bool? sobrescrever, ArquivoRecursoDto dto)
        {
            return await restService.PutRecurso(caminhoRelatorio, criarDiretorio, sobrescrever, dto);
        }

        public async Task<UnidadeRelatorioRecursoDto> Put(string caminhoRelatorio, bool criarDiretorio, bool? sobrescrever, UnidadeRelatorioRecursoDto dto)
        {
            return await restService.CriarRecursoRelatorioComId(caminhoRelatorio, criarDiretorio, sobrescrever, dto);
        }

        public async Task<OpcoesRelatorioRecursoDto> Put(string caminhoRelatorio, bool criarDiretorio, bool? sobrescrever, OpcoesRelatorioRecursoDto dto)
        {
            return await restService.PutRecurso(caminhoRelatorio, criarDiretorio, sobrescrever, dto);
        }

        public async Task<DominioRecursoDto> Put(string caminhoRelatorio, bool criarDiretorio, bool? sobrescrever, DominioRecursoDto dto)
        {
            return await restService.PutRecurso(caminhoRelatorio, criarDiretorio, sobrescrever, dto);
        }

        public async Task<ConexaoXMLARecursoDto> Put(string caminhoRelatorio, bool criarDiretorio, bool? sobrescrever, ConexaoXMLARecursoDto dto)
        {
            return await restService.PutRecurso(caminhoRelatorio, criarDiretorio, sobrescrever, dto);
        }

        public async Task<ConexaoMondrianRecursoDto> Put(string caminhoRelatorio, bool criarDiretorio, bool? sobrescrever, ConexaoMondrianRecursoDto dto)
        {
            return await restService.PutRecurso(caminhoRelatorio, criarDiretorio, sobrescrever, dto);
        }

        public async Task<ConexaoSeguraMondrianRecursoDto> Put(string caminhoRelatorio, bool criarDiretorio, bool? sobrescrever, ConexaoSeguraMondrianRecursoDto dto)
        {
            return await restService.PutRecurso(caminhoRelatorio, criarDiretorio, sobrescrever, dto);
        }

        public async Task<ConexaoOlapRecursoDto> Put(string caminhoRelatorio, bool criarDiretorio, bool? sobrescrever, ConexaoOlapRecursoDto dto)
        {
            return await restService.PutRecurso(caminhoRelatorio, criarDiretorio, sobrescrever, dto);
        }

        public async Task<MondrianXMLARecursoDto> Put(string caminhoRelatorio, bool criarDiretorio, bool? sobrescrever, MondrianXMLARecursoDto dto)
        {
            return await restService.PutRecurso(caminhoRelatorio, criarDiretorio, sobrescrever, dto);
        }

        public async Task<DetalhesRecursoDto> Put(string caminhoRelatorio, string caminhoConteudo, bool criarDiretorio, bool? sobrescrever)
        {
            return await restService.PutRecurso(caminhoRelatorio, caminhoConteudo, criarDiretorio, sobrescrever);
        }

        public async Task<UnidadeRelatorioRecursoDto> PutTopicoDominio(string caminhoRelatorio, bool criarDiretorio, bool? sobrescrever, UnidadeRelatorioRecursoDto dto)
        {
            return await restService.PutRecursoTopicoDominio(caminhoRelatorio, criarDiretorio, sobrescrever, dto);
        }
    }
}
