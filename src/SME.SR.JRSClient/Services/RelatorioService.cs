using Refit;
using SME.SR.Infra.Dtos.Requisicao;
using SME.SR.JRSClient.Grupos;
using SME.SR.JRSClient.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.JRSClient.Services
{
    public class RelatorioService : IRelatorioService
    {
        private readonly Configuracoes configuracoes;

        public RelatorioService(Configuracoes configuracoes)
        {
            this.configuracoes = configuracoes ?? throw new System.ArgumentNullException(nameof(configuracoes));
        }

        public async Task GetRelatorioSincrono(ExecutarRelatorioSincronoDto Dto)
        {
            var restService = RestService.For<IReports>(configuracoes.UrlBase);

            await restService.GetRelatorioSincrono(Dto);
        }
    }
}
