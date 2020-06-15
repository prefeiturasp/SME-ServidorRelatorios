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
                        IgnorarPaginacao = true,
                        Paginas = null,
                        Parametros = parametrosDoDto
                    };


                    SentrySdk.AddBreadcrumb("Obtendo jSessionId...", "6 - GerarRelatorioAssincronoCommandHandler");

                    var jsessionId = await loginService.ObterTokenAutenticacao(configuration.GetSection("ConfiguracaoJasper:Username").Value, configuration.GetSection("ConfiguracaoJasper:Password").Value);

                    SentrySdk.AddBreadcrumb($"jSessionId = {jsessionId}", "6 - GerarRelatorioAssincronoCommandHandler");


                    SentrySdk.AddBreadcrumb("Solicitando relatório...", "6 - GerarRelatorioAssincronoCommandHandler");


                    var retorno = await execucaoRelatorioService.SolicitarRelatorio(post, jsessionId);
                    var exportacaoId = retorno?.Exports?.FirstOrDefault()?.Id;

                    SentrySdk.AddBreadcrumb($"Exportação Id = {exportacaoId}", "6 - GerarRelatorioAssincronoCommandHandler");

                    if (exportacaoId != null)
                    {
                        var dadosRelatorio = new DadosRelatorioDto(retorno.RequestId, exportacaoId.Value, request.CodigoCorrelacao, jsessionId);

                        servicoFila.PublicaFila(new PublicaFilaDto(dadosRelatorio, RotasRabbit.FilaWorkerRelatorios, RotasRabbit.RotaRelatoriosProcessando, null, request.CodigoCorrelacao));

                        SentrySdk.AddBreadcrumb("Sucesso na publicação da fila Processando", "6 - GerarRelatorioAssincronoCommandHandler");

                        SentrySdk.CaptureMessage("6 - GerarRelatorioAssincronoCommandHandler ");

                        return await Task.FromResult(true);
                    }

                    SentrySdk.AddBreadcrumb("Erro na geração", "6 - GerarRelatorioAssincronoCommandHandler");

                    SentrySdk.CaptureMessage("6 - GerarRelatorioAssincronoCommandHandler ");

                    return await Task.FromResult(false);
                }
                catch (Exception ex)
                {
                    SentrySdk.CaptureException(ex);
                    throw ex;
                }       
        }

        private static ParametrosRelatorioDto ObterParametrosRelatorio(string dados)
        {
            var dadosParaEnvioArray = new List<string>() { dados };
            var parametroDto = new ParametroDto() { Nome = "jsonString", Valor = dadosParaEnvioArray.ToArray() };
            var parametrosDoDto = new ParametrosRelatorioDto();
            var parametrosDto = new List<ParametroDto>();

            parametrosDto.Add(parametroDto);
            parametrosDoDto.ParametrosRelatorio = parametrosDto.ToArray();
            return parametrosDoDto;
        }
    }
}
