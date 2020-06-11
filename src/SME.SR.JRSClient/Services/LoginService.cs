using Refit;
using SME.SR.JRSClient.Grupos;
using SME.SR.JRSClient.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SME.SR.JRSClient.Services
{
    public class LoginService : ServiceBase<IInfra>, ILoginService
    {
        public LoginService(Configuracoes configuracoes) : base(configuracoes)
        {

        }

        public LoginService(HttpClient httpClient, Configuracoes configuracoes) : base(httpClient, configuracoes)
        {
            
        }

        public async Task<string> ObterTokenAutenticacao(string login, string senha)
        {
            var resposta = await restService.GetLoginAsync(login, senha);

            if (!resposta.IsSuccessStatusCode)
                return string.Empty;

            IEnumerable<string> cookies=null;
            resposta.Headers?.TryGetValues("Set-Cookie",out cookies);
            if (cookies==null || !cookies.Any())
                return string.Empty;
            
            var jSessionId = cookies.FirstOrDefault(a => a.Contains("JSESSIONID"));
            if (!string.IsNullOrEmpty(jSessionId))
                return jSessionId.Substring(11, 32);

            return string.Empty;
        }
    }
}
