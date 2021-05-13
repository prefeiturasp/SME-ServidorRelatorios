using MediatR;
using Microsoft.Extensions.Configuration;
using Sentry;
using SME.SR.Infra;
using SME.SR.Infra.Dtos.Requisicao;
using SME.SR.Infra.Utilitarios;
using SME.SR.JRSClient.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class GerarRelatorioAssincronoCommandHandler : IRequestHandler<GerarRelatorioAssincronoCommand, bool>
    {
        private readonly IExecucaoRelatorioService execucaoRelatorioService;
        private readonly IServicoFila servicoFila;
        private readonly ILoginService loginService;
        private readonly IConfiguration configuration;

        public GerarRelatorioAssincronoCommandHandler(IExecucaoRelatorioService execucaoRelatorioService,
                                                      IServicoFila servicoFila,
                                                      ILoginService loginService,
                                                      IConfiguration configuration)
        {
            this.execucaoRelatorioService = execucaoRelatorioService ?? throw new System.ArgumentNullException(nameof(execucaoRelatorioService));
            this.servicoFila = servicoFila ?? throw new ArgumentNullException(nameof(servicoFila));
            this.loginService = loginService ?? throw new ArgumentNullException(nameof(loginService));
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<bool> Handle(GerarRelatorioAssincronoCommand request, CancellationToken cancellationToken)
        {                       
            try
            {                    
                ParametrosRelatorioDto parametrosDoDto = ObterParametrosRelatorio(request.Dados);

                var post = new ExecucaoRelatorioRequisicaoDto()
                {
                    UnidadeRelatorioUri = request.CaminhoRelatorio,
                    Async = false,
                    SalvarSnapshot = false,
                    FormatoSaida = request.Formato.Name(),
                    Interativo = false,
                    IgnorarPaginacao = false,
                    Paginas = null,
                    Parametros = parametrosDoDto
                };


                SentrySdk.CaptureMessage("6.1 - Obtendo jSessionId...");

                var jsessionId = await loginService.ObterTokenAutenticacao(configuration.GetSection("ConfiguracaoJasper:Username").Value, configuration.GetSection("ConfiguracaoJasper:Password").Value);

                SentrySdk.CaptureMessage($"6.2 - jSessionId = {jsessionId}");


                SentrySdk.CaptureMessage("6.3 - Solicitando relatório...");


                var retorno = await execucaoRelatorioService.SolicitarRelatorio(post, jsessionId);
                var exportacaoId = retorno?.Exports?.FirstOrDefault()?.Id;

                SentrySdk.CaptureMessage($"6.4 - Exportação Id = {exportacaoId}");

                if (exportacaoId != null)
                {
                    var dadosRelatorio = new DadosRelatorioDto(retorno.RequestId, exportacaoId.Value, request.CodigoCorrelacao, jsessionId);
                    var publicacaoFila = new PublicaFilaDto(dadosRelatorio, RotasRabbit.RotaRelatoriosProcessando, null, request.CodigoCorrelacao);

                    servicoFila.PublicaFila(publicacaoFila);

                    var jsonPublicaFila = UtilJson.ConverterApenasCamposNaoNulos(publicacaoFila);
                    Console.WriteLine(jsonPublicaFila);

                    SentrySdk.CaptureMessage("6.5 - Sucesso na publicação da fila: " + publicacaoFila.Rota);
                    return true;
                }

                SentrySdk.CaptureMessage("6.6 - Erro na geração");

                return false;
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw ex;
            }
        }

        private static ParametrosRelatorioDto ObterParametrosRelatorio(string dados)
        {
            var dadosParaEnvioArray = new List<string>();
            if (!string.IsNullOrWhiteSpace(dados))
                dadosParaEnvioArray.Add(dados);
            else
                return null;

            var parametroDto = new ParametroDto() { Nome = "jsonString", Valor = dadosParaEnvioArray.ToArray() };
            var parametrosDoDto = new ParametrosRelatorioDto();
            var parametrosDto = new List<ParametroDto>();

            parametrosDto.Add(parametroDto);
            parametrosDoDto.ParametrosRelatorio = parametrosDto.ToArray();
            return parametrosDoDto;
        }
    }
}
