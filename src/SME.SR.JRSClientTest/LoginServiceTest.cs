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
        LoginService loginService = new LoginService(new JRSClient.Configuracoes
        {
            JasperLogin = "admin",
            JasperPassword = "bitnami",
            UrlBase = "http://localhost:8080"
        });

        [Fact]
        public void ObterTokenAutenticacaoTest()
        {
            ObterTokenAutenticacaoTestAync(loginService).Wait();
        }

        private async Task ObterTokenAutenticacaoTestAync(LoginService loginService)
        {
            string response = await loginService.ObterReportStatus();
            Assert.NotNull(response);
        }
    }
}
