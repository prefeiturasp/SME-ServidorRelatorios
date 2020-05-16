using Refit;
using SME.SR.Infra.Dtos;
using SME.SR.JRSClient.Grupos;
using SME.SR.JRSClient.Interfaces;
using System.Threading.Tasks;

namespace SME.SR.JRSClient.Services
{
    public class InformacaoServidorService : ServiceBase<IInfra>, IInformacaoServidorService
    {
        public InformacaoServidorService(Configuracoes configuracoes) : base(configuracoes)
        {
        }

        public  async Task<InformacaoServidorRespostaDto> Obter()
        {
            return await restService.GetInformacaoServidorAsync();            
        }        
    }
}
