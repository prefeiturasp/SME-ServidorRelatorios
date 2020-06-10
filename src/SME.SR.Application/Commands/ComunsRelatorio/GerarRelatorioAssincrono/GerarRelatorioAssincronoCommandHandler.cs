using MediatR;
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

        public GerarRelatorioAssincronoCommandHandler(IExecucaoRelatorioService execucaoRelatorioService,
                                                      IServicoFila servicoFila)
        {
            this.execucaoRelatorioService = execucaoRelatorioService ?? throw new System.ArgumentNullException(nameof(execucaoRelatorioService));
            this.servicoFila = servicoFila ?? throw new ArgumentNullException(nameof(servicoFila));
        }
        public async Task<bool> Handle(GerarRelatorioAssincronoCommand request, CancellationToken cancellationToken)
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

            var retorno = await execucaoRelatorioService.PostAsync(post);
            var exportacaoId = retorno.Exports?.FirstOrDefault()?.Id;
            if (exportacaoId != null)
            {
                var dadosRelatorio = new DadosRelatorioDto(retorno.RequestId, exportacaoId.Value, Guid.NewGuid(), retorno.JSessionId);

                servicoFila.PublicaFila(new PublicaFilaDto(dadosRelatorio, RotasRabbit.FilaWorkerRelatorios, RotasRabbit.RotaRelatoriosProcessando));

                return await Task.FromResult(true);
            }
            return await Task.FromResult(false);
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
