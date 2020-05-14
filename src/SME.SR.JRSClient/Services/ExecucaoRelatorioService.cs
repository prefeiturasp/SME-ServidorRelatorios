using Refit;
using SME.SR.Infra.Dtos;
using SME.SR.JRSClient.Grupos;
using System.Threading.Tasks;

namespace SME.SR.JRSClient.Services
{
    public class ExecucaoRelatorioService
    {
        private readonly Configuracoes configuracoes;

        public ExecucaoRelatorioService(Configuracoes configuracoes)
        {
            this.configuracoes = configuracoes ?? throw new System.ArgumentNullException(nameof(configuracoes));
        }
        public async Task<ExecucaoRelatorioRespostaDto> Post(ExecucaoRelatorioRequisicaoDto request)
        {
            var restService = RestService.For<IReports>(configuracoes.UrlBase);

            return await restService.PostExecucoesRelatoriosAsync(request);
        }
    }
}
