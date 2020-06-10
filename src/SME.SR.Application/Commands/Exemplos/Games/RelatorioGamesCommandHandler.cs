using MediatR;
using Newtonsoft.Json;
using SME.SR.Infra.Dtos.Requisicao;
using SME.SR.JRSClient.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
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
                var dadosParaRelatorioSerializado = @"";
                var dadosParaEnvioArray = new List<string>() { dadosParaRelatorioSerializado };
                dadosParaEnvioArray[0] = dadosParaRelatorioSerializado;

                var parametroDto = new ParametroDto() { Nome = "jsonString", Valor = dadosParaEnvioArray.ToArray() };

                var parametrosDoDto = new ParametrosRelatorioDto();

                var parametrosDto = new List<ParametroDto>();

                parametrosDto.Add(parametroDto);
                parametrosDoDto.ParametrosRelatorio = parametrosDto.ToArray();

                var post = new ExecucaoRelatorioRequisicaoDto()
                {
                    UnidadeRelatorioUri = "/sme/sgp/RelatorioBoletim/RelatorioBoletim",
                    Async = true,
                    SalvarSnapshot = false,
                    FormatoSaida = "pdf",
                    Interativo = false,
                    IgnorarPaginacao = true,
                    Paginas = null,
                    Parametros = parametrosDoDto
                };

                JsonConvert.SerializeObject(post);

                var retorno = await execucaoRelatorioService.PostAsync(post);

                //var exportacaoId = Guid.Parse(retorno.Exportacoes?.FirstOrDefault()?.Exportacao?.Id);
                //TODO ENFILEIRAR COM DADOS DA EXPORTACAO





                //var teste = await execucaoRelatorioService.ObterSaida(retorno.RequisicaoId, exportacaoId);
                //var teddste = await execucaoRelatorioService.ObterDetalhes(retorno.RequisicaoId, exportacaoId);


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
