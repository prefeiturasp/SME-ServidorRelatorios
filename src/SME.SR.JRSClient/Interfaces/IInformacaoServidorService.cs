using SME.SR.Infra.Dtos;
using System.Threading.Tasks;

namespace SME.SR.JRSClient.Interfaces
{
    public interface IInformacaoServidorService
    {
        Task<InformacaoServidorRespostaDto> Obter();
    }
}
