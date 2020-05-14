using Refit;
using SME.SR.Infra.Dtos;
using System.Threading.Tasks;

namespace SME.SR.JRSClient.Grupos
{
    public interface IInfra
    {
        [Headers("Accept: application/json")]
        [Get("/jasperserver/rest_v2/serverInfo")]
        Task<InformacaoServidorRespostaDto> GetInformacaoServidorAsync();

        [Headers("Accept: application/json")]
        [Get("/jasperserver/rest_v2/login")]
        Task<ApiResponse<string>> GetLoginAsync(string j_username, string j_password);
    }
}
