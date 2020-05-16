using Refit;
using SME.SR.JRSClient.Grupos;
using SME.SR.JRSClient.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace SME.SR.JRSClient.Services
{
    public class LoginService : ServiceBase<IInfra>, ILoginService
    {
        public LoginService(Configuracoes configuracoes)  : base(configuracoes)
        {
            
        }
        public async Task<string> ObterTokenAutenticacao(string login, string senha)
        {
            var resposta = await restService.GetLoginAsync(login, senha);

            if (resposta.IsSuccessStatusCode)
            {
                var cookies = resposta.Headers.GetValues("Set-Cookie");
                if (cookies.Any())
                {
                    var jSessionId = cookies.FirstOrDefault(a => a.Contains("JSESSIONID"));
                    if (!string.IsNullOrEmpty(jSessionId))
                        return jSessionId;
                }                
            }
            return string.Empty;
        }

        public async Task<string> ObterReportStatus()
        {   
            var restService = RestService.For<IReports>(configuracoes.UrlBase, settings);

            return await restService.GetStatusAsync();            
            
        }
    }
}
