using SME.SR.JRSClient.Services;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Moq.Protected;

namespace SME.SR.JRSClientTest
{
    public class LoginServiceTest
    {
        private readonly JRSClient.Configuracoes _settings = new JRSClient.Configuracoes
        {
            JasperLogin = "user",
            JasperPassword = "bitnami",
            UrlBase = "http://localhost:8080"
        };

        [Fact]
        public void DeveInstaciarOServicoDeLogin()
        {
            // Arrange & Act
            var loginService = new LoginService(_settings);

            // Assert
            Assert.NotNull(loginService);
        }

        [Fact]
        public async Task DeveAutenticarNoServidorComSucesso()
        {
            // Arrange
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(() =>
                {
                    var response = new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent("")
                    };
                    response.Headers.Add("Set-Cookie", "JSESSIONID=1234567890123456789012345678; Path=/");
                    return response;
                })
                .Verifiable();

            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri(_settings.UrlBase)
            };

            var loginService = new LoginService(httpClient, _settings);

            // Act
            var token = await loginService.ObterTokenAutenticacao(
                _settings.JasperLogin,
                _settings.JasperPassword
            );

            // Assert
            Assert.NotEmpty(token);
            handlerMock.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            );
        }
    }
}
