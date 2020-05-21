using SME.SR.JRSClient.Services;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xunit;

namespace SME.SR.JRSClientTest
{
    public class LoginServiceTest
    {
        // TODO Evitar instancia compartilhada na execução dos caso de testes
        static JRSClient.Configuracoes Settings = new JRSClient.Configuracoes
        {
            JasperLogin = "user",
            JasperPassword = "bitnami",
            UrlBase = "http://localhost:8080"
        };

        [Fact]
        public void DeveInstaciarOServicoDeLoginEAutenticarNoServidor()
        {
            LoginService loginService = new LoginService(Settings);
            Assert.NotNull(loginService);

            string token = loginService.ObterTokenAutenticacao(
                Settings.JasperLogin, 
                Settings.JasperPassword
            ).Result;
            Assert.NotEmpty(token);
        }
    }
}
