using Refit;
using SME.SR.Infra.Dtos.Requisicao;
using SME.SR.Infra.Dtos.Resposta;
using SME.SR.JRSClient.Grupos;
using SME.SR.JRSClient.Interfaces;
using System;
using System.Threading.Tasks;

namespace SME.SR.JRSClient.Services
{
    public class ExecucaoRelatorioService : ServiceBase<IReports>, IExecucaoRelatorioService
    {
        public ExecucaoRelatorioService(Configuracoes configuracoes) : base(configuracoes)
        {

        }

        public async Task<string> InterromperRelatoriosTarefas(Guid requisicaoId)
        {
            var restService = RestService.For<IReports>(configuracoes.UrlBase);

            var retorno = await restService.PutInterromperRelatoriosTarefas(ObterCabecalhoAutenticacaoBasica(), requisicaoId);

            return default;
        }

        public async Task<string> ObterAnexos(Guid requisicaoId, Guid exportacaoId, string nomeArquivo)
        {
            var restService = RestService.For<IReports>(configuracoes.UrlBase);

            var retorno = await restService.GetAnexosRelatorio(ObterCabecalhoAutenticacaoBasica(), requisicaoId, exportacaoId, nomeArquivo);

            return default;
        }

        public async Task<DetalhesExecucaoRelatorioRespostaDto> ObterDetalhes(Guid requisicaoId)
        {
            var restService = RestService.For<IReports>(configuracoes.UrlBase);

            var retorno = await restService.GetDetalhesExecucaoRelatorio(ObterCabecalhoAutenticacaoBasica(), requisicaoId);

            return default;
        }

        public async Task<string> ObterPool(Guid requisicaoId)
        {
            var restService = RestService.For<IReports>(configuracoes.UrlBase);

            var retorno = await restService.GetPoolExecucaoRelatorio(ObterCabecalhoAutenticacaoBasica(), requisicaoId);

            return default;
        }

        public async Task<string> ObterPoolExportacao(Guid requisicaoId, Guid exportacaoId)
        {
            var restService = RestService.For<IReports>(configuracoes.UrlBase);

            var retorno = await restService.GetPoolExecucaoExportacao(ObterCabecalhoAutenticacaoBasica(), requisicaoId, exportacaoId);

            return default;
        }

        public async Task<RelatoriosTarefasEmAndamentoRespostaDto> ObterRelatoriosTarefasEmAndamento(RelatoriosTarefasEmAndamentoRequisicaoDto requisicao)
        {
            var restService = RestService.For<IReports>(configuracoes.UrlBase);
            var retorno = await restService.GetRelatoriosTarefasEmAndamento(ObterCabecalhoAutenticacaoBasica(), requisicao);
            return retorno.Content;
        }

        public async Task<string> ObterSaida(Guid requisicaoId, Guid exportacaoId)
        {
            var restService = RestService.For<IReports>(configuracoes.UrlBase);

            var retorno = await restService.GetSaidaRelatorio(ObterCabecalhoAutenticacaoBasica(), requisicaoId, exportacaoId);

            return default;
        }

        public async Task<ExecucaoRelatorioRespostaDto> PostAsync(ExecucaoRelatorioRequisicaoDto requisicao)
        {
            var restService = RestService.For<IReports>(configuracoes.UrlBase);

           return await restService.PostExecucaoRelatorioAsync(ObterCabecalhoAutenticacaoBasica(), requisicao);
        }

        public async Task<ExportacaoRelatorioRespostaDto> PostExportacao(Guid requisicaoId, ExportacaoRelatorioRequisicaoDto requisicao)
        {
            var restService = RestService.For<IReports>(configuracoes.UrlBase);

            var retorno = await restService.PostExportacaoRelatorioAsync(ObterCabecalhoAutenticacaoBasica(), requisicaoId, requisicao);

            return default;
        }

        public async Task<string> PostModificarParametros(Guid requisicaoId, ModificarParametrosRelatorioRequisicaoDto requisicao)
        {
            var restService = RestService.For<IReports>(configuracoes.UrlBase);

            var retorno = await restService.PostModificarParametrosRelatorio(ObterCabecalhoAutenticacaoBasica(), requisicaoId, requisicao);

            return default;
        }
    }
}
