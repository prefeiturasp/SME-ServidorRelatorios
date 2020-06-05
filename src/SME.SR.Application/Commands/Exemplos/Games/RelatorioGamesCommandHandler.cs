using MediatR;
using Newtonsoft.Json;
using SME.SR.Infra.Dtos.Requisicao;
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
        private readonly IRelatorioService relatorioService;

        public RelatorioGamesCommandHandler(IExecucaoRelatorioService execucaoRelatorioService, IRelatorioService relatorioService)
        {
            this.execucaoRelatorioService = execucaoRelatorioService ?? throw new ArgumentNullException(nameof(execucaoRelatorioService));
            this.relatorioService = relatorioService ?? throw new ArgumentNullException(nameof(relatorioService));
        }

        public async Task<bool> Handle(RelatorioGamesCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var dadosParaRelatorioSerializado = JsonConvert.SerializeObject(request);

                var dadosParaEnvioArray = new string[10];
                dadosParaEnvioArray[0] = dadosParaRelatorioSerializado;

                var parametroDto = new ParametroDto() { Nome = "relatorioGames", Valor = dadosParaEnvioArray };

                var parametrosDoDto = new ParametrosRelatorioDto();

                var parametrosDto = new List<ParametroDto>();
                parametrosDto.Add(parametroDto);

                var retorno = await execucaoRelatorioService.PostAsync(new ExecucaoRelatorioRequisicaoDto()
                {
                    Async = true,
                    FormatoSaida = "PDF",
                    UnidadeRelatorioUri = "/sme_sgp/teste"
                });


                var teste = await execucaoRelatorioService.PostExportacao(retorno.RequisicaoId, new ExportacaoRelatorioRequisicaoDto
                {
                    AnexosPrefixo = "teste",
                    FormatoSaida = "pdf"
                });


                //var retorno = await relatorioService.GetRelatorioSincrono(new RelatorioSincronoDto

                //retorno.URIRelatorio()

                //{
                //    CaminhoRelatorio = "/sme_sgp/teste",
                //    Formato = Enumeradores.FormatoEnum.Pdf,
                //});
                //var teste = await execucaoRelatorioService.ObterDetalhes(retorno.RequisicaoId);

            }
            catch (Exception ex)
            {

                throw;
            }
            return true;
        }
    }
}
