using MediatR;
using Microsoft.Extensions.Configuration;
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
        private readonly IMediator mediator;

        public GerarRelatorioAssincronoCommandHandler(IExecucaoRelatorioService execucaoRelatorioService,
                                                      IServicoFila servicoFila,
                                                      ILoginService loginService,
                                                      IConfiguration configuration,
                                                      IMediator mediator)
        {
            this.execucaoRelatorioService = execucaoRelatorioService ?? throw new System.ArgumentNullException(nameof(execucaoRelatorioService));
            this.servicoFila = servicoFila ?? throw new ArgumentNullException(nameof(servicoFila));
            this.loginService = loginService ?? throw new ArgumentNullException(nameof(loginService));
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
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

                var jsessionId = await loginService.ObterTokenAutenticacao(configuration.GetSection("ConfiguracaoJasper:Username").Value, configuration.GetSection("ConfiguracaoJasper:Password").Value);

                var retorno = await execucaoRelatorioService.SolicitarRelatorio(post, jsessionId);
                var exportacaoId = retorno?.Exports?.FirstOrDefault()?.Id;

                if (exportacaoId != null)
                {
                    var dadosRelatorio = new DadosRelatorioDto(retorno.RequestId, exportacaoId.Value, request.CodigoCorrelacao, jsessionId);
                    var publicacaoFila = new PublicaFilaDto(dadosRelatorio, request.RotaProcessando, ExchangeRabbit.WorkerRelatorios, request.CodigoCorrelacao);

                    await servicoFila.PublicaFila(publicacaoFila);

                    var jsonPublicaFila = UtilJson.ConverterApenasCamposNaoNulos(publicacaoFila);
                    Console.WriteLine(jsonPublicaFila);

                    return true;
                }

                if (retorno != null)
                    await RegistraErro($"6.6 - Erro na geração  / {retorno.Status}");

                await RegistraErro("6.6 - Erro na geração");

                return false;
            }
            catch (Exception ex)
            {
                await RegistraErro($"6.6 - Erro na geração: {ex.Message}, [{ex}]");
                throw ex;
            }
        }

        private async Task RegistraErro(string erro)
        {
            await mediator.Send(new SalvarLogViaRabbitCommand(erro, LogNivel.Critico));
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
