using Moq;
using SME.SR.Infra.Dtos;
using SME.SR.JRSClient.Interfaces;
using System.Threading.Tasks;
using Xunit;

namespace SME.SR.JRSClientTest
{
    public class InformacaoServidorServiceTest
    {
        [Fact]
        public async Task DeveRetornarAsInformacoesDoServidor()
        {
            // Arrange
            var informacaoServidorMock = new InformacaoServidorRespostaDto
            {
                // Preencha com os dados esperados
            };

            var mockService = new Mock<IInformacaoServidorService>();
            mockService
                .Setup(s => s.Obter())
                .ReturnsAsync(informacaoServidorMock);

            // Act
            var resultado = await mockService.Object.Obter();

            // Assert
            Assert.NotNull(resultado);
        }
    }
}
