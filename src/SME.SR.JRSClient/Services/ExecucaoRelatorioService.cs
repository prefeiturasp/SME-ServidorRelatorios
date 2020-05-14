using Refit;
using SME.SR.Infra.Dtos.Requisicao.ExecucaoRelatorio.GetRelatoriosTarefasEmAndamento;
using SME.SR.Infra.Dtos.Requisicao.ExecucaoRelatorio.PostExecucaoRelatorioAsync;
using SME.SR.Infra.Dtos.Requisicao.ExecucaoRelatorio.PostExportacaoRelatorioAsync;
using SME.SR.Infra.Dtos.Requisicao.ExecucaoRelatorio.PostModificarParametrosRelatorio;
using SME.SR.Infra.Dtos.Resposta.ExecucaoRelatorio.GetDetalhesExecucaoRelatorio;
using SME.SR.Infra.Dtos.Resposta.ExecucaoRelatorio.GetRelatoriosTarefasEmAndamento;
using SME.SR.Infra.Dtos.Resposta.ExecucaoRelatorio.PostExecucaoRelatorioAsync;
using SME.SR.Infra.Dtos.Resposta.ExecucaoRelatorio.PostExportacaoRelatorioAsync;
using SME.SR.JRSClient.Grupos;
using SME.SR.JRSClient.Interfaces;
using System;
using System.Threading.Tasks;

namespace SME.SR.JRSClient.Services
{
    public class ExecucaoRelatorioService : ServiceBase, IExecucaoRelatorioService
    {
        public ExecucaoRelatorioService(Configuracoes configuracoes) : base(configuracoes)
        {

        }

        public async Task<string> InterromperRelatoriosTarefas(Guid requisicaoId)
        {
            var restService = RestService.For<IReports>(configuracoes.UrlBase);

            return await restService.PutInterromperRelatoriosTarefas(ObterCabecalhoAutenticacaoBasica(), requisicaoId);
        }

        public async Task<string> ObterAnexos(Guid requisicaoId, Guid exportacaoId, string nomeArquivo)
        {
            var restService = RestService.For<IReports>(configuracoes.UrlBase);

            return await restService.GetAnexosRelatorio(ObterCabecalhoAutenticacaoBasica(), requisicaoId, exportacaoId, nomeArquivo);
        }

        public async Task<DetalhesExecucaoRelatorioRespostaDto> ObterDetalhes(Guid requisicaoId)
        {
            var restService = RestService.For<IReports>(configuracoes.UrlBase);

            return await restService.GetDetalhesExecucaoRelatorio(ObterCabecalhoAutenticacaoBasica(), requisicaoId);
        }

        public async Task<string> ObterPool(Guid requisicaoId)
        {
            var restService = RestService.For<IReports>(configuracoes.UrlBase);

            return await restService.GetPoolExecucaoRelatorio(ObterCabecalhoAutenticacaoBasica(), requisicaoId);
        }

        public async Task<string> ObterPoolExportacao(Guid requisicaoId, Guid exportacaoId)
        {
            var restService = RestService.For<IReports>(configuracoes.UrlBase);

            return await restService.GetPoolExecucaoExportacao(ObterCabecalhoAutenticacaoBasica(), requisicaoId, exportacaoId);
        }

        public async Task<RelatoriosTarefasEmAndamentoRespostaDto> ObterRelatoriosTarefasEmAndamento(RelatoriosTarefasEmAndamentoRequisicaoDto requisicao)
        {
            var restService = RestService.For<IReports>(configuracoes.UrlBase);

            return await restService.GetRelatoriosTarefasEmAndamento(ObterCabecalhoAutenticacaoBasica(), requisicao);
        }

        public async Task<string> ObterSaida(Guid requisicaoId, Guid exportacaoId)
        {
            var restService = RestService.For<IReports>(configuracoes.UrlBase);

            return await restService.GetSaidaRelatorio(ObterCabecalhoAutenticacaoBasica(), requisicaoId, exportacaoId);
        }

        public async Task<ExecucaoRelatorioRespostaDto> PostAsync(ExecucaoRelatorioRequisicaoDto requisicao)
        {
            var restService = RestService.For<IReports>(configuracoes.UrlBase);

            return await restService.PostExecucaoRelatorioAsync(ObterCabecalhoAutenticacaoBasica(), requisicao);
        }

        public async Task<ExportacaoRelatorioRespostaDto> PostExportacao(Guid requisicaoId, ExportacaoRelatorioRequisicaoDto requisicao)
        {
            var restService = RestService.For<IReports>(configuracoes.UrlBase);

            return await restService.PostExportacaoRelatorioAsync(ObterCabecalhoAutenticacaoBasica(), requisicaoId, requisicao);
        }

        public async Task<string> PostModificarParametros(Guid requisicaoId, ModificarParametrosRelatorioRequisicaoDto requisicao)
        {
            var restService = RestService.For<IReports>(configuracoes.UrlBase);

            return await restService.PostModificarParametrosRelatorio(ObterCabecalhoAutenticacaoBasica(), requisicaoId, requisicao);
        }
    }
}
