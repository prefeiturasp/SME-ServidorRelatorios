using Refit;
using SME.SR.Infra.Dtos;
using SME.SR.JRSClient.Grupos;
using SME.SR.JRSClient.Interfaces;
using System.Threading.Tasks;

namespace SME.SR.JRSClient.Requisicao
{
    public class InformacaoServidorService : IInformacaoServidorService
    {
        private readonly Configuracoes configuracoes;

        public InformacaoServidorService(Configuracoes configuracoes)
        {
            this.configuracoes = configuracoes ?? throw new System.ArgumentNullException(nameof(configuracoes));
        }
        public  async Task<InformacaoServidorRespostaDto> Obter()
        {
            var restService = RestService.For<IInfra>(configuracoes.UrlBase);

            return await restService.GetInformacaoServidorAsync();            
        }        
    }
}
