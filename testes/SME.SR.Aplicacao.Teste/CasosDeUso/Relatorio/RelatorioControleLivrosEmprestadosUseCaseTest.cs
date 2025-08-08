using MediatR;
using Moq;
using SME.SR.Application.Commands.CDEP.GerarRelatorioControleLivrosEmprestadosAnalitico;
using SME.SR.Application.Commands.CDEP.GerarRelatorioControleLivrosEmprestadosSintetico;
using SME.SR.Application.UseCases;
using SME.SR.Infra;

namespace SME.SR.Aplicacao.Teste.CasosDeUso.Relatorio
{
    public class RelatorioControleLivrosEmprestadosUseCaseTests
    {
        private readonly Mock<IMediator> mediatorMock;
        private readonly RelatorioControleLivrosEmprestadosUseCase useCase;

        public RelatorioControleLivrosEmprestadosUseCaseTests()
        {
            mediatorMock = new Mock<IMediator>();
            useCase = new RelatorioControleLivrosEmprestadosUseCase(mediatorMock.Object);
        }

        [Fact]
        public async Task Executar_DeveChamarMediatorParaRelatorioSintetico_QuandoFiltroForSintetico()
        {
            // Arrange
            var filtro = new FiltroRelatorioControleLivro { Modelo = ModeloRelatorio.Sintetico };
            var filtroDto = new FiltroRelatorioSincronoDto { Mensagem = Newtonsoft.Json.JsonConvert.SerializeObject(filtro) };
            var resultadoEsperado = "RelatorioSinteticoGerado";

            mediatorMock.Setup(m => m.Send(It.IsAny<GerarRelatorioControleLivrosEmprestadosSinteticoCommand>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(resultadoEsperado);

            // Act
            var resultado = await useCase.Executar(filtroDto);

            // Assert
            mediatorMock.Verify(m => m.Send(It.Is<GerarRelatorioControleLivrosEmprestadosSinteticoCommand>(c => c.Filtros.Modelo == ModeloRelatorio.Sintetico), It.IsAny<CancellationToken>()), Times.Once);
            Assert.Equal(resultadoEsperado, resultado);
        }

        [Fact]
        public async Task Executar_DeveChamarMediatorParaRelatorioAnalitico_QuandoFiltroForAnalitico()
        {
            // Arrange
            var filtro = new FiltroRelatorioControleLivro { Modelo = ModeloRelatorio.Analitico };
            var filtroDto = new FiltroRelatorioSincronoDto { Mensagem = Newtonsoft.Json.JsonConvert.SerializeObject(filtro) };
            var resultadoEsperado = "RelatorioAnaliticoGerado";

            mediatorMock.Setup(m => m.Send(It.IsAny<GerarRelatorioControleLivrosEmprestadosAnaliticoCommand>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(resultadoEsperado);

            // Act
            var resultado = await useCase.Executar(filtroDto);

            // Assert
            mediatorMock.Verify(m => m.Send(It.Is<GerarRelatorioControleLivrosEmprestadosAnaliticoCommand>(c => c.Filtros.Modelo == ModeloRelatorio.Analitico), It.IsAny<CancellationToken>()), Times.Once);
            Assert.Equal(resultadoEsperado, resultado);
        }

        [Fact]
        public async Task Executar_DeveRetornarStringVazia_QuandoModeloForInvalido()
        {
            // Arrange
            var filtro = new FiltroRelatorioControleLivro { Modelo = (ModeloRelatorio)99 }; // Modelo inválido
            var filtroDto = new FiltroRelatorioSincronoDto { Mensagem = Newtonsoft.Json.JsonConvert.SerializeObject(filtro) };

            // Act
            var resultado = await useCase.Executar(filtroDto);

            // Assert
            mediatorMock.Verify(m => m.Send(It.IsAny<IRequest<string>>(), It.IsAny<CancellationToken>()), Times.Never);
            Assert.Equal(string.Empty, resultado);
        }

        [Fact]
        public async Task Construtor_DeveLancarArgumentNullException_QuandoMediatorForNulo()
        {
            // Arrange & Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => Task.Run(() => new RelatorioControleLivrosEmprestadosUseCase(null)));
        }
    }
}
