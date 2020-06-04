using MediatR;
using Newtonsoft.Json;
using SME.SR.Infra.Dtos;
using SME.SR.Infra.Dtos.Requisicao;
using SME.SR.Infra.Enumeradores;
using SME.SR.JRSClient.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class RelatorioGamesCommandHandler : IRequestHandler<RelatorioGamesCommand, bool>
    {
        private readonly IExecucaoRelatorioService execucaoRelatorioService;

        public RelatorioGamesCommandHandler(IExecucaoRelatorioService execucaoRelatorioService)
        {
            this.execucaoRelatorioService = execucaoRelatorioService ?? throw new ArgumentNullException(nameof(execucaoRelatorioService));
        }

        public async Task<bool> Handle(RelatorioGamesCommand request, CancellationToken cancellationToken)
        {
            //var dadosParaEnvio = new Dictionary<string, string>();
            var dadosParaRelatorioSerializado = JsonConvert.SerializeObject(request);

            //dadosParaEnvio.Add("relatorioGames", dadosParaRelatorioSerializado);
            var dadosParaEnvioArray = new string[0];
            dadosParaEnvioArray[0] = dadosParaRelatorioSerializado;

            var parametroDto = new ParametroDto() { Nome = "relatorioGames", Valor = dadosParaEnvioArray };

            var parametrosDoDto = new Infra.Dtos.Requisicao.ParametrosRelatorioDto();
            
            var parametrosDto = new List<ParametroDto>();
            parametrosDto.Add(parametroDto);

            var retorno = await execucaoRelatorioService.PostAsync(new Infra.Dtos.Requisicao.ExecucaoRelatorioRequisicaoDto()
            {
                Async = true,
                FormatoSaida = "PDF",
                Parametros = parametrosDoDto,
                UnidadeRelatorioUri = "/testes/jrsclient/relatorio_exemplo_games.jrxml/relatorio_exemplo_games.jrxml"

            });
            
            //retorno.URIRelatorio()

            //var relatorio = await relatorioService.GetRelatorioSincrono(new RelatorioSincronoDto
            //{
            //    CaminhoRelatorio = "/testes/jrsclient/relatorio_exemplo_games.jrxml/relatorio_exemplo_games.jrxml",
            //    Formato = Enumeradores.FormatoEnum.Pdf,
            //    ControlesDeEntrada = dadosParaEnvio
            //});

            return true;
        }
    }
}
