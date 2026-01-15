using MediatR;
using Moq;
using Newtonsoft.Json;
using SME.SR.Application.Commands.CDEP.GerarRelatorioTitulosMaisPesquisados;
using SME.SR.Application.UseCases;
using SME.SR.Infra;
using SME.SR.Infra.CDEP;
using SME.SR.Infra.Dtos.Relatorios.CDEP;
using System.Text;

namespace SME.SR.Aplicacao.Teste.CasosDeUso.Relatorio
{
    public class RelatorioTitulosMaisPesquisadosUseCaseTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly RelatorioTitulosMaisPesquisadosUseCase _useCase;
        public RelatorioTitulosMaisPesquisadosUseCaseTestes()
        {
            _mediatorMock = new Mock<IMediator>();
            _useCase = new RelatorioTitulosMaisPesquisadosUseCase(_mediatorMock.Object);
        }

        [Fact]
        public async Task Executar_Deve_Chamar_Mediator_Send_Uma_Vez()
        {
            // Arrange
            var filtros = new FiltroRelatorioSincronoDto
            {
                Mensagem = JsonConvert.SerializeObject(new FiltroRelatorioTitulosMaisPesquisados
                {
                    Usuario = "Teste Usuario",
                    DataFim = DateTime.Now,
                    DataInicio = DateTime.Now.AddMonths(-1),
                    TipoAcervos = new List<TipoAcervo> { TipoAcervo.DocumentacaoTextual, TipoAcervo.Fotografico },
                    UsuarioRF = "123456"
                })
            };

            // Act
            await _useCase.Executar(filtros);

            // Assert
            _mediatorMock.Verify(m => m.Send(It.IsAny<GerarRelatorioTitulosMaisPesquisadosCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public void Construtor_Com_Mediator_Nulo_Deve_Lancar_Argument_Null_Exception()
        {
            Assert.Throws<ArgumentNullException>(() => new RelatorioTitulosMaisPesquisadosUseCase(null));
        }

        [Fact]
        public async Task Executar_Deve_Retornar_MemoryStream_Quando_Houver_Dados()
        {
            // Arrange
            var filtros = new FiltroRelatorioSincronoDto
            {
                Mensagem = JsonConvert.SerializeObject(new FiltroRelatorioTitulosMaisPesquisados
                {
                    Usuario = "Teste Usuario",
                    DataFim = DateTime.Now,
                    DataInicio = DateTime.Now.AddMonths(-1),
                    TipoAcervos = new List<TipoAcervo> { TipoAcervo.DocumentacaoTextual, TipoAcervo.Fotografico },
                    UsuarioRF = "123456"
                })
            };
            var memoryStreamMock = new System.IO.MemoryStream(Encoding.UTF8.GetBytes("Test Stream Content"));
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GerarRelatorioTitulosMaisPesquisadosCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(memoryStreamMock);
            // Act
            var resultado = await _useCase.Executar(filtros);
            // Assert
            Assert.NotNull(resultado);
            Assert.IsType<MemoryStream>(resultado);
            Assert.True(resultado.Length > 0);
        }

        [Fact]
        public async Task Executar_Deve_Lancar_NegocioException_Quando_Nao_Houver_Dados()
        {
            // Arrange
            var filtros = new FiltroRelatorioSincronoDto
            {
                Mensagem = JsonConvert.SerializeObject(new FiltroRelatorioTitulosMaisPesquisados
                {
                    Usuario = "Teste Usuario",
                    DataFim = DateTime.Now,
                    DataInicio = DateTime.Now.AddMonths(-1),
                    TipoAcervos = new List<TipoAcervo> { TipoAcervo.DocumentacaoTextual, TipoAcervo.Fotografico },
                    UsuarioRF = "123456"
                })
            };
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GerarRelatorioTitulosMaisPesquisadosCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new NegocioException("Nenhum dado encontrado para os filtros informados."));
            // Act & Assert
            await Assert.ThrowsAsync<NegocioException>(() => _useCase.Executar(filtros));
        }
    }
}