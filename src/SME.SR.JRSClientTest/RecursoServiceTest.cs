using SME.SR.Infra.Dtos;
using SME.SR.JRSClient.Services;
using SME.SR.JRSClientTest.Mock;
using System;
using System.IO;
using System.Collections.Generic;
using Refit;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Moq.Protected;

namespace SME.SR.JRSClientTest
{
    public class RecursoServiceTest
    {
        private readonly JRSClient.Configuracoes _settings = new JRSClient.Configuracoes
        {
            JasperLogin = "user",
            JasperPassword = "bitnami",
            UrlBase = "http://localhost:8080"
        };

        [Fact]
        public void DeveCriarUmaUnidadeRelatorio()
        {
            // Arrange
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<System.Threading.CancellationToken>()
                )
                .ReturnsAsync(() =>
                {
                    var response = new HttpResponseMessage
                    {
                        StatusCode = System.Net.HttpStatusCode.OK,
                        Content = new StringContent(
                            "{\"id\":1,\"titulo\":\"abstract_book_cover.jrxml\",\"jrxml\":{\"arquivo\":{\"tipo\":\"jrxml\",\"titulo\":\"abstract_book_cover.jrxml\",\"conteudoBase64\":\"test\"}}}"
                        )
                    };
                    response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                    return response;
                })
                .Verifiable();

            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri(_settings.UrlBase)
            };

            var recursoService = new RecursoService(httpClient, _settings);

            // Act
            var relatorio = recursoService.CriarRelatorio(
                "/testes/jrsclient/abstract_book_cover.jrxml",
                true,
                true,
                new UnidadeRelatorioRecursoDto
                {
                    Titulo = "abstract_book_cover.jrxml",
                    JRXML = new JRXMLRecursoDto
                    {
                        Arquivo = new ArquivoJRXMLRecursoDto
                        {
                            Tipo = "jrxml",
                            Titulo = "abstract_book_cover.jrxml",
                            ConteudoBase64 = RecursoServiceStaticMock.AbstractBookCoverReportContentBase64
                        }
                    }
                }
            ).Result;

            // Assert
            Assert.NotNull(relatorio);
            handlerMock.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<System.Threading.CancellationToken>()
            );
        }

        [Fact]
        public void DeveObterDetalhesDoRelatorio()
        {
            // Arrange
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<System.Threading.CancellationToken>()
                )
                .ReturnsAsync(() =>
                {
                    var response = new HttpResponseMessage
                    {
                        StatusCode = System.Net.HttpStatusCode.OK,
                        Content = new StringContent(
                            "{\"nome\":\"abstract_book_cover.jrxml\",\"resourceType\":\"jrxml\",\"expanded\":true}"
                        )
                    };
                    response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                    return response;
                })
                .Verifiable();

            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri(_settings.UrlBase)
            };

            var service = new RecursoService(httpClient, _settings);

            // Act
            var detalhes = service.ObterDetalhesRecurso(
                "/testes/jrsclient/abstract_book_cover.jrxml",
                true
            ).Result;

            // Assert
            Assert.NotNull(detalhes);
            handlerMock.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<System.Threading.CancellationToken>()
            );
        }
    }
}
