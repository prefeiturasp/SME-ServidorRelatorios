using SME.SR.Infra;
using SME.SR.Infra.Dtos;
using SME.SR.JRSClient.Services;
using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Moq.Protected;

namespace SME.SR.JRSClientTest
{
    public class RelatorioServiceTest
    {
        private readonly JRSClient.Configuracoes _settings = new JRSClient.Configuracoes
        {
            JasperLogin = "user",
            JasperPassword = "bitnami",
            UrlBase = "http://localhost:8080"
        };

        [Fact]
        public async Task DeveExecutarUmRelatorioSincrono()
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
                        StatusCode = System.Net.HttpStatusCode.OK,
                        Content = new StreamContent(new MemoryStream(new byte[] { 0x25, 0x50, 0x44, 0x46 }))
                    };
                    response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");
                    return response;
                });

            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri(_settings.UrlBase)
            };
            var relatorioService = new RelatorioService(httpClient, _settings);

            // Act
            var relatorio = await relatorioService.GetRelatorioSincrono(new RelatorioSincronoDto
            {
                CaminhoRelatorio = "/testes/jrsclient/abstract_book_cover.jrxml/abstract_book_cover.jrxml",
                Formato = TipoFormatoRelatorio.Pdf
            });

            // Assert
            Assert.NotNull(relatorio);
            handlerMock.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            );
        }
    }
}
