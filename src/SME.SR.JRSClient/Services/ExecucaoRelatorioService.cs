﻿using Microsoft.Extensions.Configuration;
using Refit;
using Sentry;
using SME.SR.Infra.Dtos.Requisicao;
using SME.SR.Infra.Dtos.Resposta;
using SME.SR.JRSClient.Extensions;
using SME.SR.JRSClient.Grupos;
using SME.SR.JRSClient.Interfaces;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SME.SR.JRSClient.Services
{
    public class ExecucaoRelatorioService : ServiceBase<IReports>, IExecucaoRelatorioService
    {
        private readonly HttpClient httpClient;
        private readonly JasperCookieHandler jasperCookieHandler;
        private readonly IConfiguration configuration;

        public ExecucaoRelatorioService(HttpClient httpClient, Configuracoes configuracoes, JasperCookieHandler jasperCookieHandler, IConfiguration configuration) : base(httpClient, configuracoes)
        {
            this.httpClient = httpClient;
            this.jasperCookieHandler = jasperCookieHandler;
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }


        public async Task<DetalhesExecucaoRelatorioRespostaDto> ObterDetalhes(Guid requisicaoId, string jSessionId)
        {
            if (!string.IsNullOrWhiteSpace(jSessionId))
                jasperCookieHandler.CookieContainer.Add(httpClient.BaseAddress, new System.Net.Cookie("JSESSIONID", jSessionId));

            var retorno = await restService.GetDetalhesExecucaoRelatorio(ObterCabecalhoAutenticacaoBasica(), jSessionId, requisicaoId);
            if (retorno.IsSuccessStatusCode)
            {
                retorno.Content.AdicionarJSessionId(jSessionId);
                return retorno.Content;
            }
            return default;
        }

        public async Task<ExecucaoRelatorioRespostaDto> SolicitarRelatorio(ExecucaoRelatorioRequisicaoDto requisicao, string jSessionId)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(jSessionId))
                    jasperCookieHandler.CookieContainer.Add(httpClient.BaseAddress, new System.Net.Cookie("JSESSIONID", jSessionId));

                SentrySdk.AddBreadcrumb("Obtendo PostExecucaoRelatorioAsync", "6.1 - ExecucaoRelatorioService");
                                
                SentrySdk.CaptureMessage($"9.8 - URI - {requisicao.UnidadeRelatorioUri}");

                var conteudoJson = $"{requisicao.Parametros.ParametrosRelatorio.FirstOrDefault().Nome} / {requisicao.Parametros.ParametrosRelatorio[0].Valor.FirstOrDefault()}";

                SentrySdk.CaptureMessage($"9.8 - Conteudo json - {conteudoJson}");

                var usuarioESenha64 = ObterUsuarioSenhaBase64();

                SentrySdk.CaptureMessage($"9.9 - Usuario e senha Base 64 - {usuarioESenha64}");

                var retorno = await restService.PostExecucaoRelatorioAsync(ObterCabecalhoAutenticacaoBasica(), requisicao);
                if (retorno.IsSuccessStatusCode)
                {
                    SentrySdk.CaptureMessage("6.1 - ExecucaoRelatorioService - Sucesso ao executar envio do relatório");
                    return retorno.Content;
                }
                SentrySdk.CaptureMessage($"9.9 - Retorno da requisição - {retorno}");

                return default;

            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw ex;
            }
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
            if (retorno.IsSuccessStatusCode)
            {
                return retorno.Content;
            }
            return default;
        }


        public async Task<string> ObterPool(Guid requisicaoId)
        {
            var restService = RestService.For<IReports>(configuracoes.UrlBase);

            var retorno = await restService.GetPoolExecucaoRelatorio(ObterCabecalhoAutenticacaoBasica(), requisicaoId);
            if (retorno.IsSuccessStatusCode)
            {
                return retorno.Content;
            }
            return default;
        }

        public async Task<string> ObterPoolExportacao(Guid requisicaoId, Guid exportacaoId)
        {
            var restService = RestService.For<IReports>(configuracoes.UrlBase);

            var retorno = await restService.GetPoolExecucaoExportacao(ObterCabecalhoAutenticacaoBasica(), requisicaoId, exportacaoId);
            if (retorno.IsSuccessStatusCode)
            {
                return retorno.Content;
            }
            return default;
        }

        public async Task<RelatoriosTarefasEmAndamentoRespostaDto> ObterRelatoriosTarefasEmAndamento(RelatoriosTarefasEmAndamentoRequisicaoDto requisicao)
        {
            var restService = RestService.For<IReports>(configuracoes.UrlBase);
            var retorno = await restService.GetRelatoriosTarefasEmAndamento(ObterCabecalhoAutenticacaoBasica(), requisicao);

            if (retorno.IsSuccessStatusCode)
            {
                return retorno.Content;
            }
            return default;
        }

        public async Task<string> ObterSaida(Guid requisicaoId, Guid exportacaoId)
        {
            var restService = RestService.For<IReports>(configuracoes.UrlBase);

            var retorno = await restService.GetSaidaRelatorio(ObterCabecalhoAutenticacaoBasica(), requisicaoId, exportacaoId);
            if (retorno.IsSuccessStatusCode)
            {
                return retorno.Content;
            }
            return default;
        }

        public async Task<ExportacaoRelatorioRespostaDto> PostExportacao(Guid requisicaoId, ExportacaoRelatorioRequisicaoDto requisicao)
        {
            var restService = RestService.For<IReports>(configuracoes.UrlBase);

            var retorno = await restService.PostExportacaoRelatorioAsync(ObterCabecalhoAutenticacaoBasica(), requisicaoId, requisicao);
            if (retorno.IsSuccessStatusCode)
            {
                return retorno.Content;
            }
            return default;
        }

        public async Task<string> PostModificarParametros(Guid requisicaoId, ModificarParametrosRelatorioRequisicaoDto requisicao)
        {
            var restService = RestService.For<IReports>(configuracoes.UrlBase);

            var retorno = await restService.PostModificarParametrosRelatorio(ObterCabecalhoAutenticacaoBasica(), requisicaoId, requisicao);
            if (retorno.IsSuccessStatusCode)
            {
                return retorno.Content;
            }
            return default;
        }
    }
}
