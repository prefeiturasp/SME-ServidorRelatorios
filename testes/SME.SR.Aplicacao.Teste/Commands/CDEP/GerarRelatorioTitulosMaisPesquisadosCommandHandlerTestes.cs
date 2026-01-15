using MediatR;
using Moq;
using SME.SR.Application.Commands.CDEP.GerarRelatorioTitulosMaisPesquisados;
using SME.SR.Application.Queries.CDEP.ObterRelatorioCDEPTitulosMaisPesquisados;
using SME.SR.Infra;
using SME.SR.Infra.Dtos.Relatorios.CDEP;

namespace SME.SR.Aplicacao.Teste.Commands.CDEP
{
    public class GerarRelatorioTitulosMaisPesquisadosCommandHandlerTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly GerarRelatorioTitulosMaisPesquisadosCommandHandler _handler;
        public GerarRelatorioTitulosMaisPesquisadosCommandHandlerTestes()
        {
            _mediatorMock = new Mock<IMediator>();
            _handler = new GerarRelatorioTitulosMaisPesquisadosCommandHandler(_mediatorMock.Object);
        }

        [Fact]
        public async Task Handle_Deve_Retornar_MemoryStream_Quando_Houver_Dados()
        {
            // Arrange
            var filtros = new FiltroRelatorioTitulosMaisPesquisados
            {
                Usuario = "Teste Usuario",
                UsuarioRF = "123456"
            };
            var command = new GerarRelatorioTitulosMaisPesquisadosCommand(filtros);
            var dadosRelatorio = new List<RelatorioTitulosMaisPesquisadosDto>
            {
                new RelatorioTitulosMaisPesquisadosDto { TermoNormalizado = "Livro A", Quantidade = 10 },
                new RelatorioTitulosMaisPesquisadosDto { TermoNormalizado = "Livro B", Quantidade = 5 }
            };
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterRelatorioCDEPTitulosMaisPesquisadosQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(dadosRelatorio);

            // Act
            var resultado = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(resultado);
            Assert.IsType<MemoryStream>(resultado);
            Assert.True(resultado.Length > 0);
        }

        [Fact]
        public async Task Handle_Deve_Lancar_NegocioException_Quando_Nao_Houver_Dados()
        {
            // Arrange
            var filtros = new FiltroRelatorioTitulosMaisPesquisados
            {
                Usuario = "Teste Usuario",
                UsuarioRF = "123456"
            };
            var command = new GerarRelatorioTitulosMaisPesquisadosCommand(filtros);
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterRelatorioCDEPTitulosMaisPesquisadosQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<RelatorioTitulosMaisPesquisadosDto>());
            
            // Act & Assert
            await Assert.ThrowsAsync<NegocioException>(() => _handler.Handle(command, CancellationToken.None));
        }
    }
}
