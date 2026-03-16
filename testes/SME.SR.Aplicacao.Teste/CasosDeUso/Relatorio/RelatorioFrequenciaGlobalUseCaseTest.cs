using MediatR;
using Moq;
using Newtonsoft.Json;
using SME.SR.Application;
using SME.SR.Application.Commands.ExportacaoExcel.GerarExcelRelatorioFrequenciaGlobal;
using SME.SR.Data;
using SME.SR.Infra;

namespace SME.SR.Aplicacao.Teste.CasosDeUso.Relatorio
{
    public class RelatorioFrequenciaGlobalUseCaseTest
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly RelatorioFrequenciaGlobalUseCase _useCase;

        public RelatorioFrequenciaGlobalUseCaseTest()
        {
            _mediatorMock = new Mock<IMediator>();
            _useCase = new RelatorioFrequenciaGlobalUseCase(_mediatorMock.Object);
        }

        [Fact]
        public async Task Executar_QuandoFormatoForPdfEFiltroTodos_DeveGerarRelatorioPdfEEnviarLogs()
        {
            // Arrange
            var filtro = CriarFiltroGlobalDto(TipoFormatoRelatorio.Pdf, ehFiltroTodos: true);
            var request = CriarFiltroRelatorio(filtro);
            var dadosRelatorio = CriarListaFrequenciaGlobalDto();

            ConfigurarMocks(dadosRelatorio);

            // Act
            await _useCase.Executar(request);

            // Assert
            _mediatorMock.Verify(m => m.Send(It.IsAny<GerarRelatorioHtmlParaPdfCommand>(), It.IsAny<CancellationToken>()), Times.Once);
            _mediatorMock.Verify(m => m.Send(It.IsAny<GerarExcelGenericoCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Executar_QuandoFormatoForXlsxEFiltroTodos_DeveGerarRelatorioXlsxEEnviarLogs()
        {
            // Arrange
            var filtro = CriarFiltroGlobalDto(TipoFormatoRelatorio.Xlsx, ehFiltroTodos: true);
            var request = CriarFiltroRelatorio(filtro);
            var dadosRelatorio = CriarListaFrequenciaGlobalDto();

            ConfigurarMocks(dadosRelatorio);

            // Act
            await _useCase.Executar(request);

            // Assert
            _mediatorMock.Verify(m => m.Send(It.IsAny<GerarExcelRelatorioFrequenciaGlobalCommand>(), It.IsAny<CancellationToken>()), Times.Once);
            _mediatorMock.Verify(m => m.Send(It.IsAny<GerarRelatorioHtmlParaPdfCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Executar_QuandoFormatoForPdfENaoForFiltroTodos_DeveGerarRelatorioPdfSemLogs()
        {
            // Arrange
            var filtro = CriarFiltroGlobalDto(TipoFormatoRelatorio.Pdf, ehFiltroTodos: false);
            var request = CriarFiltroRelatorio(filtro);
            var dadosRelatorio = CriarListaFrequenciaGlobalDto();

            ConfigurarMocks(dadosRelatorio);

            // Act
            await _useCase.Executar(request);

            // Assert
            _mediatorMock.Verify(m => m.Send(It.IsAny<SalvarLogViaRabbitCommand>(), It.IsAny<CancellationToken>()), Times.Never);
            _mediatorMock.Verify(m => m.Send(It.IsAny<GerarRelatorioHtmlParaPdfCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Executar_QuandoFiltroRetornarNuloOuVazio_DeveLancarNegocioException()
        {
            // Arrange
            var filtro = CriarFiltroGlobalDto(TipoFormatoRelatorio.Pdf, ehFiltroTodos: false);
            var request = CriarFiltroRelatorio(filtro);

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterRelatorioDeFrequenciaGlobalQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<FrequenciaGlobalDto>());

            // Act & Assert
            var excecao = await Assert.ThrowsAsync<NegocioException>(() => _useCase.Executar(request));
            Assert.Equal("Não foi possível localizar informações com os filtros selecionados", excecao.Message);
        }

        [Fact]
        public async Task Executar_QuandoFormatoForInvalido_DeveLancarNegocioException()
        {
            // Arrange
            var filtro = CriarFiltroGlobalDto((TipoFormatoRelatorio)99, ehFiltroTodos: false);
            var request = CriarFiltroRelatorio(filtro);
            var dadosRelatorio = CriarListaFrequenciaGlobalDto();

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterRelatorioDeFrequenciaGlobalQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(dadosRelatorio);

            // Act & Assert
            var excecao = await Assert.ThrowsAsync<NegocioException>(() => _useCase.Executar(request));
            Assert.Contains("Não foi possível exportar este relátorio para o formato", excecao.Message);
        }

        [Fact]
        public async Task Executar_QuandoQueryFalhar_DevePropagarExcecao()
        {
            // Arrange
            var filtro = CriarFiltroGlobalDto(TipoFormatoRelatorio.Pdf, ehFiltroTodos: false);
            var request = CriarFiltroRelatorio(filtro);

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterRelatorioDeFrequenciaGlobalQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ArgumentNullException());

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _useCase.Executar(request));
        }

        #region Metodos Privados Auxiliares

        private void ConfigurarMocks(List<FrequenciaGlobalDto> dadosRelatorio)
        {
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterRelatorioDeFrequenciaGlobalQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(dadosRelatorio);

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterTurmaPorCodigoQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Turma { Nome = "Minha Turma" });

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterDreUePorDreCodigoQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new DreUe { DreNome = "DRE TESTE", UeNome = "UE TESTE" });

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterDrePorCodigoQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Dre() { Abreviacao = "DRE-TESTE", Nome = "DIRETORIA REGIONAL DE EDUCACAO TESTE" });
        }

        private FiltroRelatorioDto CriarFiltroRelatorio(FiltroFrequenciaGlobalDto filtro)
        {
            return new FiltroRelatorioDto
            {
                CodigoCorrelacao = Guid.NewGuid(),
                Mensagem = JsonConvert.SerializeObject(filtro)
            };
        }

        private FiltroFrequenciaGlobalDto CriarFiltroGlobalDto(TipoFormatoRelatorio formato, bool ehFiltroTodos)
        {
            return new FiltroFrequenciaGlobalDto
            {
                AnoLetivo = 2025,
                CodigoDre = ehFiltroTodos ? "-99" : "108300",
                CodigoUe = "-99",
                Modalidade = Modalidade.EJA,
                UsuarioNome = "Usuario Teste",
                UsuarioRf = "1234567",
                CodigosTurmas = new List<string> { "-99" },
                MesesReferencias = new List<string> { "-99" },
                TipoFormatoRelatorio = formato
            };
        }

        private List<FrequenciaGlobalDto> CriarListaFrequenciaGlobalDto()
        {
            return new List<FrequenciaGlobalDto>
            {
                new FrequenciaGlobalDto
                {
                    DreCodigo = "108300",
                    SiglaDre = "DRE-TESTE",
                    UeCodigo = "123",
                    UeNome = "ESCOLA TESTE",
                    Mes = 1,
                    TurmaCodigo = "T1",
                    Turma = "TURMA 01",
                    CodigoEOL = "EOL123",
                    Estudante = "ALUNO TESTE",
                    PercentualFrequencia = 95
                }
            };
        }

        #endregion
    }
}
