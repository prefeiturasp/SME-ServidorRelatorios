using MediatR;
using Moq;
using SME.SR.Application;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Aplicacao.Teste.CasosDeUso.Relatorio
{
    public class RelatorioDevolutivasUseCaseTeste
    {
        private readonly Mock<IMediator> mediatorMock;
        private readonly RelatorioDevolutivasUseCase useCase;

        public RelatorioDevolutivasUseCaseTeste()
        {
            mediatorMock = new Mock<IMediator>();
            useCase = new RelatorioDevolutivasUseCase(mediatorMock.Object);
        }

        [Fact]
        public async Task Executar_DeveGerarRelatorio_ComTodosFiltros()
        {
            // Arrange
            var filtroDto = new FiltroRelatorioDto
            {
                CodigoCorrelacao = "123",
                ObterObjetoFiltro = () => new FiltroRelatorioDevolutivasDto
                {
                    Ano = 2025,
                    UeId = 1,
                    Turmas = new List<long> { 1 },
                    Bimestres = new List<int> { 1 },
                    UsuarioNome = "Usuário Teste",
                    UsuarioRF = "RF123",
                    ExibirDetalhes = true,
                    ComponenteCurricular = 10
                }
            };

            mediatorMock.Setup(m => m.Send(It.IsAny<ObterUeComDrePorIdQuery>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(new Ue { Nome = "UE Teste", Dre = new Dre { Abreviacao = "DRE" } });

            mediatorMock.Setup(m => m.Send(It.IsAny<ObterTurmaPorIdQuery>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(new Turma { NomeRelatorio = "Turma 1" });

            mediatorMock.Setup(m => m.Send(It.IsAny<VerificarSeParametroEstaAtivoQuery>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(new ParametroSistemaDto { Ano = 2025, Ativo = true });

            mediatorMock.Setup(m => m.Send(It.IsAny<ObterDevolutivasQuery>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(new List<TurmasDevolutivasDto> { new TurmasDevolutivasDto { NomeTurma = "Turma 1" } });

            mediatorMock.Setup(m => m.Send(It.IsAny<GerarRelatorioHtmlParaPdfCommand>(), It.IsAny<CancellationToken>()))
                        .Returns(Task.CompletedTask);

            // Act
            await useCase.Executar(filtroDto);

            // Assert
            mediatorMock.Verify(m => m.Send(It.IsAny<ObterDevolutivasQuery>(), It.IsAny<CancellationToken>()), Times.Once);
            mediatorMock.Verify(m => m.Send(It.IsAny<GerarRelatorioHtmlParaPdfCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ObterBimestresFiltro_DeveRetornarTodosQuandoBimestreForMinus99()
        {
            // Arrange
            var bimestres = new List<int> { -99 };

            // Act
            var resultado = useCase.ObterBimestresFiltro(bimestres);

            // Assert
            Assert.Equal(new List<int> { 1, 2, 3, 4 }, resultado);
        }

        [Fact]
        public async Task ObterTurmas_DeveRetornarVazioQuandoTurmaForMinus99()
        {
            // Arrange
            var turmas = new List<long> { -99 };

            // Act
            var resultado = useCase.ObterTurmas(turmas);

            // Assert
            Assert.Empty(resultado);
        }

        [Fact]
        public async Task ObterBimestres_DeveFormatarCorretamente()
        {
            // Arrange
            var bimestres = new List<int> { 1, 2 };

            // Act
            var resultado = useCase.ObterBimestres(bimestres);

            // Assert
            Assert.Equal("1º,2º", resultado);
        }

        [Fact]
        public async Task ObterBimestres_DeveRetornarVazioQuandoConterMinus99()
        {
            // Arrange
            var bimestres = new List<int> { -99, 1 };

            // Act
            var resultado = useCase.ObterBimestres(bimestres);

            // Assert
            Assert.Equal("", resultado);
        }

        [Fact]
        public async Task ObterTurma_DeveRetornarTodasQuandoListaVazia()
        {
            // Act
            var resultado = await useCase.ObterTurma(new List<long>());

            // Assert
            Assert.Equal("Todas", resultado);
        }

        [Fact]
        public async Task ObterTurma_DeveRetornarNomeTurmaQuandoUmItem()
        {
            // Arrange
            var turma = new Turma { NomeRelatorio = "Turma Teste" };
            mediatorMock.Setup(m => m.Send(It.IsAny<ObterTurmaPorIdQuery>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(turma);

            // Act
            var resultado = await useCase.ObterTurma(new List<long> { 1 });

            // Assert
            Assert.Equal("Turma Teste", resultado);
        }

        [Fact]
        public async Task UtilizarNovoLayout_DeveRetornarTrueQuandoParametroAtivo()
        {
            // Arrange
            mediatorMock.Setup(m => m.Send(It.IsAny<VerificarSeParametroEstaAtivoQuery>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(new ParametroSistemaDto { Ano = 2025, Ativo = true });

            // Act
            var resultado = await useCase.UtilizarNovoLayout(2025);

            // Assert
            Assert.True(resultado);
        }

        [Fact]
        public async Task UtilizarNovoLayout_DeveRetornarFalseQuandoParametroInativo()
        {
            // Arrange
            mediatorMock.Setup(m => m.Send(It.IsAny<VerificarSeParametroEstaAtivoQuery>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(new ParametroSistemaDto { Ano = 2025, Ativo = false });

            // Act
            var resultado = await useCase.UtilizarNovoLayout(2025);

            // Assert
            Assert.False(resultado);
        }

        [Fact]
        public async Task Executar_DeveTratarBimestresETurmasEspeciais()
        {
            // Arrange
            var filtroDto = new FiltroRelatorioDto
            {
                CodigoCorrelacao = "123",
                ObterObjetoFiltro = () => new FiltroRelatorioDevolutivasDto
                {
                    Ano = 2025,
                    UeId = 1,
                    Turmas = new List<long> { -99 },
                    Bimestres = new List<int> { -99 },
                    UsuarioNome = "Teste",
                    UsuarioRF = "RF",
                    ExibirDetalhes = false,
                    ComponenteCurricular = 10
                }
            };

            mediatorMock.Setup(m => m.Send(It.IsAny<ObterUeComDrePorIdQuery>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(new Ue { Nome = "UE", Dre = new Dre { Abreviacao = "DRE" } });

            mediatorMock.Setup(m => m.Send(It.IsAny<VerificarSeParametroEstaAtivoQuery>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(new ParametroSistemaDto { Ano = 2020, Ativo = true });

            mediatorMock.Setup(m => m.Send(It.IsAny<ObterDevolutivasQuery>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(new List<TurmasDevolutivasDto>());

            mediatorMock.Setup(m => m.Send(It.IsAny<GerarRelatorioHtmlParaPdfCommand>(), It.IsAny<CancellationToken>()))
                        .Returns(Task.CompletedTask);

            // Act
            await useCase.Executar(filtroDto);

            // Assert
            mediatorMock.Verify(m => m.Send(It.IsAny<ObterDevolutivasQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
